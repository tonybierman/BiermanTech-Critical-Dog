using BiermanTech.CriticalDog.Data;
using System.Linq.Expressions;

namespace BiermanTech.CriticalDog.Reports.Domain;

public interface ISubjectRecordFilterProvider
{
    Dictionary<string, Expression<Func<SubjectRecord, bool>>> Filters { get; }

    IEnumerable<IEnumerable<string>> GetFacetKeys();
    Expression<Func<SubjectRecord, bool>> GetFilter(string key);
}