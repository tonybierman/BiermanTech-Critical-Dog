using BiermanTech.CriticalDog.Data;
using UniversalReportCore;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class PagedSubjectRecordQueryParameters : PagedQueryParameters<SubjectRecord>
    {
        public PagedSubjectRecordQueryParameters(IReportColumnDefinition[] columns, int? pageIndex, string? sort, int? itemsPerPage, int[]? cohortIds,
            Func<IQueryable<SubjectRecord>, IQueryable<SubjectRecord>>? additionalFilter = null,
            Func<IQueryable<SubjectRecord>, Task<Dictionary<string, dynamic>>>? aggregateLogic = null,
            Func<IQueryable<SubjectRecord>, Task<Dictionary<string, dynamic>>>? metaLogic = null) :
            base(columns, pageIndex, sort, itemsPerPage, cohortIds, additionalFilter, aggregateLogic, metaLogic)
        {
        }
    }
}
