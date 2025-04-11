using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports.Domain;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    public class KennelQueryProvider : PagedSubjectRecordQueryProvider
    {
        public override string Slug => "Kennel";

        public KennelQueryProvider(AppDbContext dbContext) : base(dbContext) { }

        public override IQueryable<SubjectRecord> EnsureReportQuery()
        {
            IQueryable<SubjectRecord> query = _dbContext.SubjectRecords;

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
