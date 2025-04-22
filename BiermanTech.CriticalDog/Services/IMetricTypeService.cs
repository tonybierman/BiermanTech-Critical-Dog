using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BiermanTech.CriticalDog.Services
{
    public interface IMetricTypeService
    {
        Task CreateMetricTypeAsync(MetricTypeInputViewModel viewModel);
        Task DeleteMetricTypeAsync(int id);
        Task<List<MetricTypeInputViewModel>> GetAllMetricTypesAsync();
        Task<MetricType> GetMetricTypeByIdAsync(int id);
        Task<MetricTypeInputViewModel> GetMetricTypeViewModelByIdAsync(int id);
        Task UpdateMetricTypeAsync(MetricTypeInputViewModel viewModel);
    }
}