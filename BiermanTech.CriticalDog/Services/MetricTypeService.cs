using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class MetricTypeService : IMetricTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MetricTypeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MetricType> GetMetricTypeByIdAsync(int id)
        {
            return await _context.MetricTypes
                .Include(m => m.ObservationDefinition)
                .Include(m => m.Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MetricTypeInputViewModel> GetMetricTypeViewModelByIdAsync(int id)
        {
            var metricType = await GetMetricTypeByIdAsync(id);
            return metricType == null ? null : _mapper.Map<MetricTypeInputViewModel>(metricType);
        }

        public async Task<List<MetricTypeInputViewModel>> GetAllMetricTypesAsync()
        {
            var metricTypes = await _context.MetricTypes.ToListAsync();
            return _mapper.Map<List<MetricTypeInputViewModel>>(metricTypes);
        }

        public async Task<SelectList> GetObservationDefinitionsSelectListAsync()
        {
            var definitions = await _context.ObservationDefinitions.ToListAsync();
            return new SelectList(definitions, nameof(ObservationDefinition.Id), nameof(ObservationDefinition.DefinitionName));
        }

        public async Task<SelectList> GetUnitsSelectListAsync()
        {
            var units = await _context.Units.ToListAsync();
            return new SelectList(units, nameof(Unit.Id), nameof(Unit.UnitName));
        }

        public async Task CreateMetricTypeAsync(MetricTypeInputViewModel viewModel)
        {
            var entity = _mapper.Map<MetricType>(viewModel);
            _context.MetricTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMetricTypeAsync(MetricTypeInputViewModel viewModel)
        {
            var metricType = await _context.MetricTypes.FindAsync(viewModel.Id);
            if (metricType == null)
            {
                throw new KeyNotFoundException($"MetricType with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, metricType);
            _context.MetricTypes.Update(metricType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMetricTypeAsync(int id)
        {
            var metricType = await _context.MetricTypes.FindAsync(id);
            if (metricType == null)
            {
                throw new KeyNotFoundException($"MetricType with ID {id} not found.");
            }

            _context.MetricTypes.Remove(metricType);
            await _context.SaveChangesAsync();
        }
    }
}