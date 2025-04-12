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
            return await _context.Subjects
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

        public async Task CreateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var entity = _mapper.Map<Subject>(viewModel);
            _context.Subjects.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var subject = await _context.Subjects.FindAsync(viewModel.Id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, subject);
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}