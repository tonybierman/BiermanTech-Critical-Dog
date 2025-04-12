using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace BiermanTech.CriticalDog.Data
{
    public partial class AppDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Subject>()
                .HasIndex(r => r.UserId);
        }
    }
}
