using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class ObservationTypeService : IObservationTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ObservationTypeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ObservationType> GetObservationTypeByIdAsync(int id)
        {
            return await _context.ObservationTypes
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ObservationTypeInputViewModel> GetObservationTypeViewModelByIdAsync(int id)
        {
            var observationType = await GetObservationTypeByIdAsync(id);
            return observationType == null ? null : _mapper.Map<ObservationTypeInputViewModel>(observationType);
        }

        public async Task<List<ObservationTypeInputViewModel>> GetAllObservationTypesAsync()
        {
            var observationTypes = await _context.ObservationTypes.ToListAsync();
            return _mapper.Map<List<ObservationTypeInputViewModel>>(observationTypes);
        }

        public async Task CreateObservationTypeAsync(ObservationTypeInputViewModel viewModel)
        {
            var entity = _mapper.Map<ObservationType>(viewModel);
            _context.ObservationTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateObservationTypeAsync(ObservationTypeInputViewModel viewModel)
        {
            var observationType = await _context.ObservationTypes.FindAsync(viewModel.Id);
            if (observationType == null)
            {
                throw new KeyNotFoundException($"ObservationType with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, observationType);
            _context.ObservationTypes.Update(observationType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteObservationTypeAsync(int id)
        {
            var observationType = await _context.ObservationTypes.FindAsync(id);
            if (observationType == null)
            {
                throw new KeyNotFoundException($"ObservationType with ID {id} not found.");
            }

            _context.ObservationTypes.Remove(observationType);
            await _context.SaveChangesAsync();
        }
    }
}