using BiermanTech.CriticalDog.Data;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class SubjectRecordQueryFactory : QueryFactory<SubjectRecord>
    {
        public SubjectRecordQueryFactory(IEnumerable<IPagedQueryProvider<SubjectRecord>> providers) : base(providers)
        {
        }
    }
}
