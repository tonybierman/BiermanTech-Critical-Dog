using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubjectService(
            AppDbContext context,
            IMapper mapper,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
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
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User must be authenticated to create a subject.");
            }

            var entity = _mapper.Map<Subject>(viewModel);

            // Set default permissions (complements AppDbContext.ApplyUserIdOnSave)
            entity.Permissions = SubjectPermissions.OwnerCanView |
                                SubjectPermissions.OwnerCanEdit |
                                SubjectPermissions.OwnerCanDelete |
                                SubjectPermissions.AdminCanView |
                                SubjectPermissions.AdminCanEdit |
                                SubjectPermissions.AdminCanDelete;

            _context.Add(entity); // UserId and audit fields set by ApplyUserIdOnSave
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var subject = await _context.Subjects // Use Subjects directly to check existence
                .FirstOrDefaultAsync(s => s.Id == viewModel.Id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {viewModel.Id} not found.");
            }

            // Check CanEdit permission
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                subject,
                "SubjectCanEdit");
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedAccessException($"User lacks permission to edit Subject with ID {viewModel.Id}.");
            }

            // Ensure immutable permissions are preserved
            viewModel.Permissions |= SubjectPermissions.OwnerCanView |
                                     SubjectPermissions.AdminCanView |
                                     SubjectPermissions.AdminCanEdit |
                                     SubjectPermissions.AdminCanDelete;

            _mapper.Map(viewModel, subject);
            _context.Update(subject);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects // Use Subjects directly to check existence
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }

            // Check CanDelete permission
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                subject,
                "SubjectCanDelete");
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedAccessException($"User lacks permission to delete Subject with ID {id}.");
            }

            _context.Remove(subject);
            return await _context.SaveChangesAsync();
        }
    }
}