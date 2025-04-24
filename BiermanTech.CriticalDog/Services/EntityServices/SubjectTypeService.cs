using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class SubjectTypeService : ISubjectTypeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SubjectTypeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SubjectType> GetSubjectTypeByIdAsync(int id)
        {
            return await _context.SubjectTypes
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SubjectTypeInputViewModel> GetSubjectTypeViewModelByIdAsync(int id)
        {
            var subjectType = await GetSubjectTypeByIdAsync(id);
            return subjectType == null ? null : _mapper.Map<SubjectTypeInputViewModel>(subjectType);
        }

        public async Task<List<SubjectTypeInputViewModel>> GetAllSubjectTypesAsync()
        {
            var subjectTypes = await _context.SubjectTypes.ToListAsync();
            return _mapper.Map<List<SubjectTypeInputViewModel>>(subjectTypes);
        }

        public async Task CreateSubjectTypeAsync(SubjectTypeInputViewModel viewModel)
        {
            var entity = _mapper.Map<SubjectType>(viewModel);
            _context.SubjectTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubjectTypeAsync(SubjectTypeInputViewModel viewModel)
        {
            var subjectType = await _context.SubjectTypes.FindAsync(viewModel.Id);
            if (subjectType == null)
            {
                throw new KeyNotFoundException($"SubjectType with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, subjectType);
            _context.SubjectTypes.Update(subjectType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubjectTypeAsync(int id)
        {
            var subjectType = await _context.SubjectTypes.FindAsync(id);
            if (subjectType == null)
            {
                throw new KeyNotFoundException($"SubjectType with ID {id} not found.");
            }

            _context.SubjectTypes.Remove(subjectType);
            await _context.SaveChangesAsync();
        }
    }
}