using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    public class KennelQueryProvider : PagedSubjectRecordQueryProvider
    {
        public override string Slug => "Kennel";
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KennelQueryProvider(AppDbContext dbContext,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService) : base(dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public override IQueryable<SubjectRecord> EnsureReportQuery()
        {
            IQueryable<SubjectRecord> query = _dbContext.GetFilteredSubjectRecords()
                .Include(a => a.Subject)
                .ThenInclude(a => a.SubjectType)
                .Include(a => a.ObservationDefinition)
                .ThenInclude(od => od.ObservationType)
                .Include(a => a.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(a => a.MetaTags);

            return query;
        }
    }
}