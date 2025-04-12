using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Services
{
    public interface IScientificDisciplineService
    {
        Task CreateDisciplineAsync(ScientificDisciplineInputViewModel viewModel);
        Task DeleteDisciplineAsync(int id);
        Task<List<ScientificDisciplineInputViewModel>> GetAllDisciplinesAsync();
        Task<ScientificDiscipline> GetDisciplineByIdAsync(int id);
        Task<ScientificDisciplineInputViewModel> GetDisciplineViewModelByIdAsync(int id);
        Task UpdateDisciplineAsync(ScientificDisciplineInputViewModel viewModel);
    }
}