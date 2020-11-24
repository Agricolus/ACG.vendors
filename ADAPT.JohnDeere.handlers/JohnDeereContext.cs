using System;
using ADAPT.JohnDeere.handlers.Model;
using Microsoft.EntityFrameworkCore;

namespace ADAPT.JohnDeere.handlers
{

    public class PostgresContext : JohnDeereContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
               => optionsBuilder.UseNpgsql("Host=192.168.1.181;Database=ACG;Username=postgres;Password=postgres");
    }

    public class JohnDeereContext : DbContext
    {
        public JohnDeereContext() { }
        public JohnDeereContext(DbContextOptions<JohnDeereContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // .. and invoke "BuildIndexesFromAnnotations"!
            // modelBuilder.BuildIndexesFromAnnotations();
        }

        public DbSet<UserToken> UsersTokens { get; set; }
    }
}
