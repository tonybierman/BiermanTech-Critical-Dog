using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IUnitService
    {
        Task CreateUnitAsync(UnitInputViewModel viewModel);
        Task DeleteUnitAsync(int id);
        Task<List<UnitInputViewModel>> GetAllUnitsAsync();
        Task<Unit> GetUnitByIdAsync(int id);
        Task<UnitInputViewModel> GetUnitViewModelByIdAsync(int id);
        Task UpdateUnitAsync(UnitInputViewModel viewModel);
    }
}