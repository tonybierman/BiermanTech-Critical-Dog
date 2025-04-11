using BiermanTech.CriticalDog.Data;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class PagedDogRecordQueryProvider : BasePagedQueryProvider<DogRecord>
    {
        protected readonly AppDbContext _dbContext;

        public PagedDogRecordQueryProvider(AppDbContext dbContext)
        {
            _dbContext = dbContext;    
        }

        public override string Slug => throw new NotImplementedException();

        protected async override Task<Dictionary<string, dynamic>> EnsureMeta(IQueryable<DogRecord> query)
        {
            return new Dictionary<string, dynamic>();
        }

        //public override IQueryable<DogRecord> EnsureAggregatePredicate(IQueryable<DogRecord> query, int[]? cohortIds)
        //{
        //    if (cohortIds == null || cohortIds.Length == 0)
        //    {
        //        return query;  // No filtering needed
        //    }

        //    // Fetch all DogRecordsIds associated with the specified cohortIds
        //    var ids = _dbContext.DogRecords
        //        .Where(p => p.DogRecordCohorts.Any(c => cohortIds.Contains(c.Id)))
        //        .Select(p => p.Id)
        //        .ToList();

        //    // Filter DogRecords where Id is not null and matches one of the ids
        //    return query.Where(entity => ids.Contains(entity.Id));
        //}

        //public override IQueryable<DogRecord> EnsureCohortPredicate(IQueryable<DogRecord> query, int cohortId)
        //{
        //    return from entity in query
        //           join cityPop in _dbContext.DogRecords on entity.Id equals cityPop.Id
        //           where cityPop.DogRecordCohorts.Any(c => c.Id == cohortId)
        //           select entity;
        //}
    }
}
