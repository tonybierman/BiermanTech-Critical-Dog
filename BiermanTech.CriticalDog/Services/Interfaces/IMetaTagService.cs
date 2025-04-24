using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IMetaTagService
    {
        Task CreateMetaTagAsync(MetaTagInputViewModel viewModel);
        Task DeleteMetaTagAsync(int id);
        Task<List<MetaTagInputViewModel>> GetAllMetaTagsAsync();
        Task<MetaTag> GetMetaTagByIdAsync(int id);
        Task<MetaTagInputViewModel> GetMetaTagViewModelByIdAsync(int id);
        Task UpdateMetaTagAsync(MetaTagInputViewModel viewModel);
    }
}