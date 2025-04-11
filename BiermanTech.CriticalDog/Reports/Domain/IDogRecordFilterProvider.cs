using BiermanTech.CriticalDog.Data;
using System.Linq.Expressions;

namespace BiermanTech.CriticalDog.Reports.Domain;

public interface IDogRecordFilterProvider
{
    Dictionary<string, Expression<Func<DogRecord, bool>>> Filters { get; }

    IEnumerable<IEnumerable<string>> GetFacetKeys();
    Expression<Func<DogRecord, bool>> GetFilter(string key);
}