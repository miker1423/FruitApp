using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.Models;

namespace FruitAppAPI.DBContexts
{
    public class AppDBContext : DbContext
    {
        public DbSet<Provider> Providers { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
    }
}
