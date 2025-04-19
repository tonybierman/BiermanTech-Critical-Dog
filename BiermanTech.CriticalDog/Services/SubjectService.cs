using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SubjectService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _context.GetFilteredSubjects()
                .Include(s => s.SubjectType)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id)
        {
            var subject = await GetSubjectByIdAsync(id);
            return subject == null ? null : _mapper.Map<SubjectInputViewModel>(subject);
        }

        public async Task<SelectList> GetSubjectTypesSelectListAsync()
        {
            var subjectTypes = await _context.SubjectTypes.ToListAsync();
            return new SelectList(subjectTypes, nameof(SubjectType.Id), nameof(SubjectType.TypeName));
        }

        public async Task<int> CreateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var entity = _mapper.Map<Subject>(viewModel);
            _context.Add(entity); // Use generic Add; UserId set by ApplyUserIdOnSave

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var subject = await _context.GetFilteredSubjects()
                .FirstOrDefaultAsync(s => s.Id == viewModel.Id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {viewModel.Id} not found or you lack permission to access it.");
            }

            _mapper.Map(viewModel, subject);
            _context.Update(subject);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteSubjectAsync(int id)
        {
            var subject = await _context.GetFilteredSubjects()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found or you lack permission to access it.");
            }

            _context.Remove(subject);
            return await _context.SaveChangesAsync();
        }
    }
}