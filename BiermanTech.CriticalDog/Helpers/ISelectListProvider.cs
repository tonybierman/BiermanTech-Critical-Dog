using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public interface ISelectListProvider
    {
        IEnumerable<SelectListItem> GetSelectListItems();
    }
}
