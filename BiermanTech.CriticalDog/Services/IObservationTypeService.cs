using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services
{
    public interface IObservationTypeService
    {
        Task CreateObservationTypeAsync(ObservationTypeInputViewModel viewModel);
        Task DeleteObservationTypeAsync(int id);
        Task<List<ObservationTypeInputViewModel>> GetAllObservationTypesAsync();
        Task<ObservationType> GetObservationTypeByIdAsync(int id);
        Task<ObservationTypeInputViewModel> GetObservationTypeViewModelByIdAsync(int id);
        Task UpdateObservationTypeAsync(ObservationTypeInputViewModel viewModel);
    }
}