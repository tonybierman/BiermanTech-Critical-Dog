using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BiermanTech.CriticalDog.Data
{
    public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base first to configure Identity tables
            base.OnModelCreating(builder);

            builder
                .UseCollation("utf8mb4_unicode_ci")
                .HasCharSet("utf8mb4");

            // Set default charset and collation for all tables
            //builder.HasCharset("utf8mb4")
            //      .HasDefaultCollation("utf8mb4_unicode_ci");

            // Explicitly set collation for each Identity entity
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                builder.Entity(entity.Name)
                       .ToTable(entity.GetTableName())
                       .HasCharSet("utf8mb4")
                       .UseCollation("utf8mb4_unicode_ci");
            }
        }
    }
}
