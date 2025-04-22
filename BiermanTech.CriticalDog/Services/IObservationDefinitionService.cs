using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface IObservationDefinitionService
    {
        Task CreateDefinitionAsync(ObservationDefinitionInputViewModel viewModel);
        Task DeleteDefinitionAsync(int id);
        Task<List<ObservationDefinitionInputViewModel>> GetAllDefinitionsAsync();
        Task<ObservationDefinition> GetDefinitionByIdAsync(int id);
        Task<ObservationDefinitionInputViewModel> GetDefinitionViewModelByIdAsync(int id);
        Task UpdateDefinitionAsync(ObservationDefinitionInputViewModel viewModel);
    }
}