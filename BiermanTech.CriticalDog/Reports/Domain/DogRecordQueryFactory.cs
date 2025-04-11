using BiermanTech.CriticalDog.Data;
using UniversalReportCore.PagedQueries;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class DogRecordQueryFactory : QueryFactory<DogRecord>
    {
        public DogRecordQueryFactory(IEnumerable<IPagedQueryProvider<DogRecord>> providers) : base(providers)
        {
        }
    }
}
