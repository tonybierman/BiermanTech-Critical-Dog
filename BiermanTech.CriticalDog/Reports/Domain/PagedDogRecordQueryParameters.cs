using BiermanTech.CriticalDog.Data;
using UniversalReportCore;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class PagedDogRecordQueryParameters : PagedQueryParameters<DogRecord>
    {
        public PagedDogRecordQueryParameters(IReportColumnDefinition[] columns, int? pageIndex, string? sort, int? itemsPerPage, int[]? cohortIds,
            Func<IQueryable<DogRecord>, IQueryable<DogRecord>>? additionalFilter = null,
            Func<IQueryable<DogRecord>, Task<Dictionary<string, dynamic>>>? aggregateLogic = null,
            Func<IQueryable<DogRecord>, Task<Dictionary<string, dynamic>>>? metaLogic = null) :
            base(columns, pageIndex, sort, itemsPerPage, cohortIds, additionalFilter, aggregateLogic, metaLogic)
        {
        }
    }
}
