using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class ObservationDefinitionService : IObservationDefinitionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ObservationDefinitionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ObservationDefinition> GetDefinitionByIdAsync(int id)
        {
            return await _context.ObservationDefinitions
                .Include(d => d.ObservationType)
                .Include(d => d.ScientificDisciplines)
                .Include(d => d.MetricTypes)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<ObservationDefinitionInputViewModel> GetDefinitionViewModelByIdAsync(int id)
        {
            var definition = await GetDefinitionByIdAsync(id);
            if (definition == null)
            {
                throw new KeyNotFoundException($"ObservationDefinition with ID {id} not found.");
            }

            return _mapper.Map<ObservationDefinitionInputViewModel>(definition);
        }

        public async Task<List<ObservationDefinitionInputViewModel>> GetAllDefinitionsAsync()
        {
            var definitions = await _context.ObservationDefinitions
                .Include(d => d.ObservationType)
                .Include(d => d.ScientificDisciplines)
                .Include(d => d.MetricTypes)
                .OrderBy(d => d.DefinitionName)
                .ToListAsync();

            return _mapper.Map<List<ObservationDefinitionInputViewModel>>(definitions);
        }

        public async Task<SelectList> GetMetricTypesSelectListAsync(IEnumerable<int>? selectedIds = null)
        {
            var items = await _context.MetricTypes
                .Where(mt => mt.IsActive == true)
                .OrderBy(mt => mt.Description)
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Description
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedIds);
        }

        public async Task<SelectList> GetObservationTypesSelectListAsync()
        {
            var types = await _context.ObservationTypes.ToListAsync();
            return new SelectList(types, nameof(ObservationType.Id), nameof(ObservationType.TypeName));
        }

        public async Task<SelectList> GetScientificDisciplinesSelectListAsync()
        {
            var disciplines = await _context.ScientificDisciplines.ToListAsync();
            return new SelectList(disciplines, nameof(ScientificDiscipline.Id), nameof(ScientificDiscipline.DisciplineName));
        }

        public async Task CreateDefinitionAsync(ObservationDefinitionInputViewModel viewModel)
        {
            var entity = _mapper.Map<ObservationDefinition>(viewModel);

            if (viewModel.SelectedScientificDisciplineIds != null)
            {
                var disciplines = await _context.ScientificDisciplines
                    .Where(d => viewModel.SelectedScientificDisciplineIds.Contains(d.Id))
                    .ToListAsync();
                if (disciplines.Count != viewModel.SelectedScientificDisciplineIds.Count)
                    throw new InvalidOperationException("One or more ScientificDiscipline IDs are invalid.");
                entity.ScientificDisciplines = disciplines;
            }
            else
            {
                entity.ScientificDisciplines = new List<ScientificDiscipline>();
            }

            if (viewModel.MetricTypeIds != null)
            {
                var metricTypes = await _context.MetricTypes
                    .Where(mt => viewModel.MetricTypeIds.Contains(mt.Id))
                    .ToListAsync();
                if (metricTypes.Count != viewModel.MetricTypeIds.Count)
                    throw new InvalidOperationException("One or more MetricType IDs are invalid.");
                entity.MetricTypes = metricTypes;
            }
            else
            {
                entity.MetricTypes = new List<MetricType>();
            }

            _context.ObservationDefinitions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDefinitionAsync(ObservationDefinitionInputViewModel viewModel)
        {
            var definition = await _context.ObservationDefinitions
                .Include(d => d.ScientificDisciplines)
                .Include(d => d.MetricTypes)
                .FirstOrDefaultAsync(d => d.Id == viewModel.Id);

            if (definition == null)
            {
                throw new KeyNotFoundException($"ObservationDefinition with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, definition);

            definition.ScientificDisciplines.Clear();
            if (viewModel.SelectedScientificDisciplineIds != null)
            {
                var disciplines = await _context.ScientificDisciplines
                    .Where(d => viewModel.SelectedScientificDisciplineIds.Contains(d.Id))
                    .ToListAsync();
                if (disciplines.Count != viewModel.SelectedScientificDisciplineIds.Count)
                    throw new InvalidOperationException("One or more ScientificDiscipline IDs are invalid.");
                definition.ScientificDisciplines = disciplines;
            }

            definition.MetricTypes.Clear();
            if (viewModel.MetricTypeIds != null)
            {
                var metricTypes = await _context.MetricTypes
                    .Where(mt => viewModel.MetricTypeIds.Contains(mt.Id))
                    .ToListAsync();
                if (metricTypes.Count != viewModel.MetricTypeIds.Count)
                    throw new InvalidOperationException("One or more MetricType IDs are invalid.");
                definition.MetricTypes = metricTypes;
            }

            _context.ObservationDefinitions.Update(definition);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDefinitionAsync(int id)
        {
            var definition = await _context.ObservationDefinitions
                .Include(d => d.ScientificDisciplines)
                .Include(d => d.MetricTypes)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (definition == null)
            {
                throw new KeyNotFoundException($"ObservationDefinition with ID {id} not found.");
            }

            // Clear many-to-many relationships
            definition.ScientificDisciplines.Clear();
            definition.MetricTypes.Clear();

            _context.ObservationDefinitions.Remove(definition);
            await _context.SaveChangesAsync();
        }
    }
}