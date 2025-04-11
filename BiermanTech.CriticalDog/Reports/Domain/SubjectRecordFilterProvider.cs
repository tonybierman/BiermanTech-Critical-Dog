using BiermanTech.CriticalDog.Data;
using System.Linq.Expressions;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class SubjectRecordFilterProvider : BaseFilterProvider<SubjectRecord>
    {
        public SubjectRecordFilterProvider() : base(new List<Facet<SubjectRecord>>
        {
            new("Sex", new()
            {
                new("Male", p => p.Subject.Sex == 0),
                new("Female", p => p.Subject.Sex == 1),
            })
        })
        { }
    }
}
