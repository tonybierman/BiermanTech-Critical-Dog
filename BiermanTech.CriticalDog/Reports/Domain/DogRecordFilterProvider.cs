using BiermanTech.CriticalDog.Data;
using System.Linq.Expressions;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class DogRecordFilterProvider : BaseFilterProvider<DogRecord>
    {
        public DogRecordFilterProvider() : base(new List<Facet<DogRecord>>
        {
            new("Sex", new()
            {
                new("Male", p => p.Dog.Sex == 0),
                new("Female", p => p.Dog.Sex == 1),
            })
        })
        { }
    }
}
