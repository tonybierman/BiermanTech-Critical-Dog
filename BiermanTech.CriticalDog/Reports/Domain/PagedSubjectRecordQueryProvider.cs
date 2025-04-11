using BiermanTech.CriticalDog.Data;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class PagedSubjectRecordQueryProvider : BasePagedQueryProvider<SubjectRecord>
    {
        protected readonly AppDbContext _dbContext;

        public PagedSubjectRecordQueryProvider(AppDbContext dbContext)
        {
            _dbContext = dbContext;    
        }

        public override string Slug => throw new NotImplementedException();

        protected async override Task<Dictionary<string, dynamic>> EnsureMeta(IQueryable<SubjectRecord> query)
        {
            return new Dictionary<string, dynamic>();
        }

        //public override IQueryable<SubjectRecord> EnsureAggregatePredicate(IQueryable<SubjectRecord> query, int[]? cohortIds)
        //{
        //    if (cohortIds == null || cohortIds.Length == 0)
        //    {
        //        return query;  // No filtering needed
        //    }

        //    // Fetch all SubjectRecordsIds associated with the specified cohortIds
        //    var ids = _dbContext.SubjectRecords
        //        .Where(p => p.SubjectRecordCohorts.Any(c => cohortIds.Contains(c.Id)))
        //        .Select(p => p.Id)
        //        .ToList();

        //    // Filter SubjectRecords where Id is not null and matches one of the ids
        //    return query.Where(entity => ids.Contains(entity.Id));
        //}

        //public override IQueryable<SubjectRecord> EnsureCohortPredicate(IQueryable<SubjectRecord> query, int cohortId)
        //{
        //    return from entity in query
        //           join cityPop in _dbContext.SubjectRecords on entity.Id equals cityPop.Id
        //           where cityPop.SubjectRecordCohorts.Any(c => c.Id == cohortId)
        //           select entity;
        //}
    }
}
