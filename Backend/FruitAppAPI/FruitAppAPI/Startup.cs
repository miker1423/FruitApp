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

using FruitAppAPI.DBContexts;
using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.Services;

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
            services.AddDbContext<AppDBContext>(options =>
            {
                if (_env.IsDevelopment())
                {
                    options.UseInMemoryDatabase("DB");
                }
                else
                {
                    options.UseSqlServer(Configuration.GetValue<string>("SQL"));
                }
            });

            services.AddScoped<IProviderService, ProviderService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(config =>
            {
                config.MapAreaRoute(
                    "api",
                    "api",
                    "{area:exists}/{controller}/{action}/{id?}");
            });
        }
    }
}
