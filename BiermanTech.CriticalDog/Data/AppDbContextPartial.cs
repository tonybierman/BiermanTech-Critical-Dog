using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Diagnostics; // For diagnostics
using Microsoft.Extensions.Logging; // Optional: for logging

namespace BiermanTech.CriticalDog.Data
{
    public partial class AppDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AppDbContext> _logger; // Optional: for logging

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            ILogger<AppDbContext> logger = null) // Optional: logger injection
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Preserve existing index
            modelBuilder.Entity<Subject>()
                .HasIndex(r => r.UserId);
        }

        public IQueryable<Subject> GetFilteredSubjects()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    _logger?.LogWarning("GetFilteredSubjects: User is null, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjects: User is null.");
                    return Subjects.Where(s => false);
                }

                if (!user.Identity.IsAuthenticated)
                {
                    _logger?.LogWarning("GetFilteredSubjects: User is not authenticated, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjects: User is not authenticated.");
                    return Subjects.Where(s => false);
                }

                string userId = _userManager.GetUserId(user);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjects: UserId is null or empty, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjects: UserId is null or empty.");
                    return Subjects.Where(s => false);
                }

                bool isAdmin = user.IsInRole("Admin");
                _logger?.LogInformation($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");

                return isAdmin
                    ? Subjects
                    : Subjects.Where(s => s.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetFilteredSubjects");
                Debug.WriteLine($"Error in GetFilteredSubjects: {ex.Message}");
                throw; // Re-throw to catch in debugger or logs
            }
        }

        public IQueryable<SubjectRecord> GetFilteredSubjectRecords()
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: User is null, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjectRecords: User is null.");
                    return SubjectRecords.Where(r => false);
                }

                if (!user.Identity.IsAuthenticated)
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: User is not authenticated, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjectRecords: User is not authenticated.");
                    return SubjectRecords.Where(r => false);
                }

                string userId = _userManager.GetUserId(user);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: UserId is null or empty, returning empty query.");
                    Debug.WriteLine("GetFilteredSubjectRecords: UserId is null or empty.");
                    return SubjectRecords.Where(r => false);
                }

                bool isAdmin = user.IsInRole("Admin");
                _logger?.LogInformation($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");

                return isAdmin
                    ? SubjectRecords
                    : SubjectRecords.Where(r => r.Subject.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetFilteredSubjectRecords");
                Debug.WriteLine($"Error in GetFilteredSubjectRecords: {ex.Message}");
                throw; // Re-throw to catch in debugger or logs
            }
        }

        public override int SaveChanges()
        {
            ApplyUserIdOnSave();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyUserIdOnSave();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyUserIdOnSave()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(user);
                if (!string.IsNullOrEmpty(userId))
                {
                    var entries = ChangeTracker.Entries<Subject>()
                        .Where(e => e.State == EntityState.Added);

                    foreach (var entry in entries)
                    {
                        entry.Entity.UserId = userId;
                    }
                }
            }
        }
    }
}