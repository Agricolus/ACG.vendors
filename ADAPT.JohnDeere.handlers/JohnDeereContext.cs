using System;
using ADAPT.JohnDeere.handlers.Model;
using Microsoft.EntityFrameworkCore;

namespace ADAPT.JohnDeere.handlers
{

    public class PostgresContext : JohnDeereContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
               => optionsBuilder.UseNpgsql("Host=192.168.1.181;Database=ACGVendors;Username=postgres;Password=postgres");
    }

    public class JohnDeereContext : DbContext
    {
        public JohnDeereContext() { }
        public JohnDeereContext(DbContextOptions<JohnDeereContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var uuidPGExtension = modelBuilder.HasPostgresExtension("uuid-ossp");
            uuidPGExtension.Entity<Machine>()
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            uuidPGExtension.Entity<DocumentFile>()
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            uuidPGExtension.Entity<Client>()
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            uuidPGExtension.Entity<Field>()
                .Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
        }

        public DbSet<UserToken> UsersTokens { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<DocumentFile> DocumentFile { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Field> Fields { get; set; }
    }
}
