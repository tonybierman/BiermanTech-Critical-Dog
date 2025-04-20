using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BiermanTech.CriticalDog.Data
{
    public partial class AppDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AppDbContext> _logger;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            ILogger<AppDbContext> logger = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>()
                .HasIndex(r => r.UserId);
        }

        public IQueryable<Subject> GetFilteredSubjects()
        {
            try
            {
                var user = _httpContextAccessor?.HttpContext?.User;
                if (user == null)
                {
                    _logger?.LogWarning("GetFilteredSubjects: User is null, returning anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjects: User is null.");
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                if (!user.Identity.IsAuthenticated)
                {
                    _logger?.LogWarning("GetFilteredSubjects: User is not authenticated, returning anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjects: User is not authenticated.");
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                string userId = _userManager.GetUserId(user);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjects: UserId is null or empty, returning anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjects: UserId is null or empty.");
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                bool isAdmin = user.IsInRole("Admin");
                _logger?.LogInformation($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");

                if (isAdmin)
                {
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AdminCanView) != 0);
                }

                return Subjects.Where(s =>
                    (s.Permissions & SubjectPermissions.AuthenticatedCanView) != 0 ||
                    ((s.Permissions & SubjectPermissions.OwnerCanView) != 0 && s.UserId == userId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetFilteredSubjects");
                Debug.WriteLine($"Error in GetFilteredSubjects: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<Subject>> GetFilteredSubjects(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjects: Provided UserId is null or empty, returning anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjects: Provided UserId is null or empty.");
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null)
                {
                    _logger?.LogWarning($"GetFilteredSubjects: No user found for UserId={userId}, returning anonymous-accessible subjects.");
                    Debug.WriteLine($"GetFilteredSubjects: No user found for UserId={userId}.");
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(identityUser, "Admin");
                _logger?.LogInformation($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjects: UserId={userId}, IsAdmin={isAdmin}");

                if (isAdmin)
                {
                    return Subjects.Where(s => (s.Permissions & SubjectPermissions.AdminCanView) != 0);
                }

                return Subjects.Where(s =>
                    (s.Permissions & SubjectPermissions.AuthenticatedCanView) != 0 ||
                    ((s.Permissions & SubjectPermissions.OwnerCanView) != 0 && s.UserId == userId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error in GetFilteredSubjects for UserId={userId}");
                Debug.WriteLine($"Error in GetFilteredSubjects for UserId={userId}: {ex.Message}");
                throw;
            }
        }

        public IQueryable<SubjectRecord> GetFilteredSubjectRecords()
        {
            try
            {
                var user = _httpContextAccessor?.HttpContext?.User;
                if (user == null)
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: User is null, returning records for anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjectRecords: User is null.");
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                if (!user.Identity.IsAuthenticated)
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: User is not authenticated, returning records for anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjectRecords: User is not authenticated.");
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                string userId = _userManager.GetUserId(user);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: UserId is null or empty, returning records for anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjectRecords: UserId is null or empty.");
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                bool isAdmin = user.IsInRole("Admin");
                _logger?.LogInformation($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");

                if (isAdmin)
                {
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AdminCanView) != 0);
                }

                return SubjectRecords.Where(r =>
                    (r.Subject.Permissions & SubjectPermissions.AuthenticatedCanView) != 0 ||
                    ((r.Subject.Permissions & SubjectPermissions.OwnerCanView) != 0 && r.Subject.UserId == userId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetFilteredSubjectRecords");
                Debug.WriteLine($"Error in GetFilteredSubjectRecords: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<SubjectRecord>> GetFilteredSubjectRecords(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _logger?.LogWarning("GetFilteredSubjectRecords: Provided UserId is null or empty, returning records for anonymous-accessible subjects.");
                    Debug.WriteLine("GetFilteredSubjectRecords: Provided UserId is null or empty.");
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser == null)
                {
                    _logger?.LogWarning($"GetFilteredSubjectRecords: No user found for UserId={userId}, returning records for anonymous-accessible subjects.");
                    Debug.WriteLine($"GetFilteredSubjectRecords: No user found for UserId={userId}.");
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AnonymousCanView) != 0);
                }

                bool isAdmin = await _userManager.IsInRoleAsync(identityUser, "Admin");
                _logger?.LogInformation($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");
                Debug.WriteLine($"GetFilteredSubjectRecords: UserId={userId}, IsAdmin={isAdmin}");

                if (isAdmin)
                {
                    return SubjectRecords.Where(r => (r.Subject.Permissions & SubjectPermissions.AdminCanView) != 0);
                }

                return SubjectRecords.Where(r =>
                    (r.Subject.Permissions & SubjectPermissions.AuthenticatedCanView) != 0 ||
                    ((r.Subject.Permissions & SubjectPermissions.OwnerCanView) != 0 && r.Subject.UserId == userId));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error in GetFilteredSubjectRecords for UserId={userId}");
                Debug.WriteLine($"Error in GetFilteredSubjectRecords for UserId={userId}: {ex.Message}");
                throw;
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
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(user);
                if (!string.IsNullOrEmpty(userId))
                {
                    var now = DateTime.UtcNow;

                    // Handle Subject entities
                    var subjectEntries = ChangeTracker.Entries<Subject>()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                    foreach (var entry in subjectEntries)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entry.Entity.UserId = userId;
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = now;
                            // Set default Permissions
                            entry.Entity.Permissions = SubjectPermissions.OwnerCanView |
                                                      SubjectPermissions.OwnerCanEdit |
                                                      SubjectPermissions.OwnerCanDelete |
                                                      SubjectPermissions.AdminCanView |
                                                      SubjectPermissions.AdminCanEdit |
                                                      SubjectPermissions.AdminCanDelete;
                        }
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedAt = now;
                    }

                    // Handle SubjectRecord entities
                    var recordEntries = ChangeTracker.Entries<SubjectRecord>()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                    foreach (var entry in recordEntries)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = now;
                        }
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedAt = now;
                    }
                }
            }
        }
    }
}