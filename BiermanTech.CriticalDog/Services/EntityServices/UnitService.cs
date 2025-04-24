using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class UnitService : IUnitService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UnitService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            return await _context.Units
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UnitInputViewModel> GetUnitViewModelByIdAsync(int id)
        {
            var unit = await GetUnitByIdAsync(id);
            return unit == null ? null : _mapper.Map<UnitInputViewModel>(unit);
        }

        public async Task<List<UnitInputViewModel>> GetAllUnitsAsync()
        {
            var units = await _context.Units.ToListAsync();
            return _mapper.Map<List<UnitInputViewModel>>(units);
        }

        public async Task CreateUnitAsync(UnitInputViewModel viewModel)
        {
            var entity = _mapper.Map<Unit>(viewModel);
            _context.Units.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUnitAsync(UnitInputViewModel viewModel)
        {
            var unit = await _context.Units.FindAsync(viewModel.Id);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Unit with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, unit);
            _context.Units.Update(unit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUnitAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                throw new KeyNotFoundException($"Unit with ID {id} not found.");
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
        }
    }
}