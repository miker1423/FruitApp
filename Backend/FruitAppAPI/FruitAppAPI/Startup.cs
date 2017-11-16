using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Neo4jClient;

using FruitAppAPI.IdConfig;
using FruitAppAPI.DBContexts;
using FruitAppAPI.Utils.ConfigModels;
using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.Services;
using FruitAppAPI.Models;
using FruitAppAPI.Utils;

namespace FruitAppAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _env;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IGraphClient graphClient = new GraphClient(new Uri(Configuration.GetValue<string>("Neo4j")));
            graphClient.Connect();

            services.AddSingleton(graphClient);

            var keyVault = new KeyVaultUtils(
                Configuration.GetValue<string>("KeyVaultClientId"),
                Configuration.GetValue<string>("KeyVaultClientSecret"),
                Configuration.GetValue<string>("KeyVaultUrl"));

            var certTask = keyVault.GetCertificate(Configuration.GetValue<string>("CertName")).GetAwaiter();
            
            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetValue<string>("SQL"));
            });

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetValue<string>("SQL"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<TableConfig>(config =>
            {
                config.ConnectionString = Configuration.GetValue<string>("TableString");
            });

            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<ICertificatesService, CertificatesService>();
            services.AddScoped<IFruitGraphService, FruitGraphService>();
            services.AddScoped<IProvidersGraphService, ProviderGraphService>();
            services.AddScoped<IOrdersService, OrdersService>();
            
            services.AddIdentityServer()
                .AddSigningCredential(certTask.GetResult())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();
                

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            MigrateDb(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseMvc(config =>
            {
                config.MapAreaRoute(
                    "api",
                    "api",
                    "{area:exists}/{controller}/{action}/{id?}");

                config.MapAreaRoute(
                    "main",
                    "main",
                    "{area:exists}/{controller}/{action}/{id?}");
            });
        }

        private void MigrateDb(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Database.Migrate();

                var context2 = serviceScope.ServiceProvider.GetRequiredService<UsersDbContext>();
                context2.Database.Migrate();
            }
        }
    }
}
