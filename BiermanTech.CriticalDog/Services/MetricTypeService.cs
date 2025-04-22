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
                .Include(m => m.ObservationDefinitions)
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
            var metricTypes = await _context.MetricTypes
                .Include(m => m.ObservationDefinitions)
                .ToListAsync();
            return _mapper.Map<List<MetricTypeInputViewModel>>(metricTypes);
        }

        public async Task<SelectList> GetObservationDefinitionsSelectListAsync()
        {
            var definitions = await _context.ObservationDefinitions
                .Where(od => od.IsActive == true)
                .OrderBy(od => od.DefinitionName)
                .ToListAsync();
            return new SelectList(definitions, nameof(ObservationDefinition.Id), nameof(ObservationDefinition.DefinitionName));
        }

        public async Task<SelectList> GetUnitsSelectListAsync()
        {
            var units = await _context.Units
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.UnitName)
                .ToListAsync();
            return new SelectList(units, nameof(Unit.Id), nameof(Unit.UnitName));
        }

        public async Task CreateMetricTypeAsync(MetricTypeInputViewModel viewModel)
        {
            var entity = _mapper.Map<MetricType>(viewModel);
            if (viewModel.ObservationDefinitionIds?.Any() == true)
            {
                var observationDefinitions = await _context.ObservationDefinitions
                    .Where(od => viewModel.ObservationDefinitionIds.Contains(od.Id))
                    .ToListAsync();
                entity.ObservationDefinitions = observationDefinitions;
            }
            _context.MetricTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMetricTypeAsync(MetricTypeInputViewModel viewModel)
        {
            var metricType = await _context.MetricTypes
                .Include(m => m.ObservationDefinitions)
                .FirstOrDefaultAsync(m => m.Id == viewModel.Id);
            if (metricType == null)
            {
                throw new KeyNotFoundException($"MetricType with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, metricType);
            metricType.ObservationDefinitions.Clear();
            if (viewModel.ObservationDefinitionIds?.Any() == true)
            {
                var observationDefinitions = await _context.ObservationDefinitions
                    .Where(od => viewModel.ObservationDefinitionIds.Contains(od.Id))
                    .ToListAsync();
                metricType.ObservationDefinitions = observationDefinitions;
            }
            _context.MetricTypes.Update(metricType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMetricTypeAsync(int id)
        {
            var metricType = await _context.MetricTypes
                .Include(m => m.ObservationDefinitions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (metricType == null)
            {
                throw new KeyNotFoundException($"MetricType with ID {id} not found.");
            }

            metricType.ObservationDefinitions.Clear();
            _context.MetricTypes.Remove(metricType);
            await _context.SaveChangesAsync();
        }
    }
}