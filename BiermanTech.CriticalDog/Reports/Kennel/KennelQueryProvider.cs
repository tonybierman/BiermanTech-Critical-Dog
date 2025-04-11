using Microsoft.EntityFrameworkCore;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports.Domain;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    public class KennelQueryProvider : PagedDogRecordQueryProvider
    {
        public override string Slug => "Kennel";

        public KennelQueryProvider(AppDbContext dbContext) : base(dbContext) { }

        public override IQueryable<DogRecord> EnsureReportQuery()
        {
            IQueryable<DogRecord> query = _dbContext.DogRecords;

            var optimizedQuery = query
                .Include(a => a.Dog)
                .Include(a => a.MetricType)
                .ThenInclude(mt => mt.ObservationDefinition)
                .Include(a => a.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(a => a.MetaTags);

            return optimizedQuery;
        }
    }
}
