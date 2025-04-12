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
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<ObservationDefinitionInputViewModel> GetDefinitionViewModelByIdAsync(int id)
        {
            var definition = await GetDefinitionByIdAsync(id);
            if (definition == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<ObservationDefinitionInputViewModel>(definition);
            viewModel.SelectedScientificDisciplineIds = definition.ScientificDisciplines
                .Select(d => d.Id)
                .ToList();
            return viewModel;
        }

        public async Task<List<ObservationDefinitionInputViewModel>> GetAllDefinitionsAsync()
        {
            var definitions = await _context.ObservationDefinitions
                .Include(d => d.ObservationType)
                .Include(d => d.ScientificDisciplines)
                .ToListAsync();
            var viewModels = _mapper.Map<List<ObservationDefinitionInputViewModel>>(definitions);
            for (int i = 0; i < definitions.Count; i++)
            {
                viewModels[i].SelectedScientificDisciplineIds = definitions[i].ScientificDisciplines
                    .Select(d => d.Id)
                    .ToList();
            }
            return viewModels;
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
            entity.ScientificDisciplines = await _context.ScientificDisciplines
                .Where(d => viewModel.SelectedScientificDisciplineIds.Contains(d.Id))
                .ToListAsync();
            _context.ObservationDefinitions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDefinitionAsync(ObservationDefinitionInputViewModel viewModel)
        {
            var definition = await _context.ObservationDefinitions
                .Include(d => d.ScientificDisciplines)
                .FirstOrDefaultAsync(d => d.Id == viewModel.Id);
            if (definition == null)
            {
                throw new KeyNotFoundException($"ObservationDefinition with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, definition);
            definition.ScientificDisciplines.Clear();
            definition.ScientificDisciplines = await _context.ScientificDisciplines
                .Where(d => viewModel.SelectedScientificDisciplineIds.Contains(d.Id))
                .ToListAsync();
            _context.ObservationDefinitions.Update(definition);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDefinitionAsync(int id)
        {
            var definition = await _context.ObservationDefinitions.FindAsync(id);
            if (definition == null)
            {
                throw new KeyNotFoundException($"ObservationDefinition with ID {id} not found.");
            }

            _context.ObservationDefinitions.Remove(definition);
            await _context.SaveChangesAsync();
        }
    }
}