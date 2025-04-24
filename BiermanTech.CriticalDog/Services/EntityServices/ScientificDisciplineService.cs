using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class ScientificDisciplineService : IScientificDisciplineService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ScientificDisciplineService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ScientificDiscipline> GetDisciplineByIdAsync(int id)
        {
            return await _context.ScientificDisciplines
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<ScientificDisciplineInputViewModel> GetDisciplineViewModelByIdAsync(int id)
        {
            var discipline = await GetDisciplineByIdAsync(id);
            return discipline == null ? null : _mapper.Map<ScientificDisciplineInputViewModel>(discipline);
        }

        public async Task<List<ScientificDisciplineInputViewModel>> GetAllDisciplinesAsync()
        {
            var disciplines = await _context.ScientificDisciplines.ToListAsync();
            return _mapper.Map<List<ScientificDisciplineInputViewModel>>(disciplines);
        }

        public async Task CreateDisciplineAsync(ScientificDisciplineInputViewModel viewModel)
        {
            var entity = _mapper.Map<ScientificDiscipline>(viewModel);
            _context.ScientificDisciplines.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDisciplineAsync(ScientificDisciplineInputViewModel viewModel)
        {
            var discipline = await _context.ScientificDisciplines.FindAsync(viewModel.Id);
            if (discipline == null)
            {
                throw new KeyNotFoundException($"ScientificDiscipline with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, discipline);
            _context.ScientificDisciplines.Update(discipline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDisciplineAsync(int id)
        {
            var discipline = await _context.ScientificDisciplines.FindAsync(id);
            if (discipline == null)
            {
                throw new KeyNotFoundException($"ScientificDiscipline with ID {id} not found.");
            }

            _context.ScientificDisciplines.Remove(discipline);
            await _context.SaveChangesAsync();
        }
    }
}