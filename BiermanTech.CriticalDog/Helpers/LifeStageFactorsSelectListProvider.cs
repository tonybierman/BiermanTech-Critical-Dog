using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Helpers
{
    public class LifeStageFactorsSelectListProvider : ISelectListProvider
    {
        public IEnumerable<SelectListItem> GetSelectListItems()
        {
            return SelectListHelper.GetLifeStageFactorsSelectList();
        }
    }
}
