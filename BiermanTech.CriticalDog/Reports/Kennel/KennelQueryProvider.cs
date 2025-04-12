using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    public class KennelQueryProvider : PagedSubjectRecordQueryProvider
    {
        public override string Slug => "Kennel";
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KennelQueryProvider(AppDbContext dbContext,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public override IQueryable<SubjectRecord> EnsureReportQuery()
        {
            // Get current user's ID
            string userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (string.IsNullOrEmpty(userId))
            {
                // Return empty query for unauthenticated users
                return _dbContext.SubjectRecords.Where(a => false);
            }

            IQueryable<SubjectRecord> query = _dbContext.SubjectRecords
                .Where(a => a.Subject.UserId == userId);

            var optimizedQuery = query
                .Include(a => a.Subject)
                .ThenInclude(a => a.SubjectType)
                .Include(a => a.ObservationDefinition)
                .Include(a => a.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(a => a.MetaTags);

            return optimizedQuery;
        }
    }
}