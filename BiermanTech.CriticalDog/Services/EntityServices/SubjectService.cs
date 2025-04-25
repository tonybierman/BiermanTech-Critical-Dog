using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BiermanTech.CriticalDog.Services.Interfaces;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(
            AppDbContext context,
            IMapper mapper,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SubjectService> logger)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<SubjectViewModel>> GetFilteredSubjectViewModelsAsync()
        {
            _logger.LogInformation("GetFilteredSubjectViewModelsAsync: Retrieving filtered subjects.");

            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var subjects = await _context.GetFilteredSubjects()
                .Include(s => s.SubjectType)
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .AsNoTracking()
                .ToListAsync();

            var viewModels = _mapper.Map<List<SubjectViewModel>>(subjects);

            _logger.LogInformation($"GetFilteredSubjectViewModelsAsync: Retrieved {subjects.Count} subjects.");

            return viewModels;
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            _logger.LogInformation($"GetSubjectByIdAsync: Retrieving Subject with ID {id}.");

            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var query = _context.GetFilteredSubjects()
                .Include(s => s.SubjectType)
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .AsNoTracking(); // Optional: Use if read-only to improve performance

            var subject = await query.FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
            {
                _logger.LogWarning($"GetSubjectByIdAsync: Subject with ID {id} not found or user lacks view permissions.");
            }
            else
            {
                _logger.LogInformation($"GetSubjectByIdAsync: Retrieved Subject ID={id}, Permissions={subject.Permissions}, UserId={subject.UserId}, MetaTagsCount={subject.MetaTags.Count}.");
            }

            return subject;
        }

        public async Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id)
        {
            var subject = await GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return null;
            }

            var viewModel = _mapper.Map<SubjectInputViewModel>(subject);

            return viewModel;
        }

        public async Task<IList<Subject>> GetFilteredSubjectsAsync()
        {
            _logger.LogInformation("GetFilteredSubjectsAsync: Retrieving filtered subjects.");
            var subjects = await _context.GetFilteredSubjects().ToListAsync();
            _logger.LogInformation($"GetFilteredSubjectsAsync: Retrieved {subjects.Count} subjects.");
            return subjects;
        }

        public async Task<int> CreateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User must be authenticated to create a subject.");
            }

            var entity = _mapper.Map<Subject>(viewModel);
            entity.MetaTags = await _context.MetaTags
                .Where(m => viewModel.SelectedMetaTagIds.Contains(m.Id))
                .ToListAsync();

            // UserId and default Permissions are set in AppDbContext.ApplyUserIdOnSave

            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var subject = await _context.Subjects
                .Include(s => s.MetaTags)
                .FirstOrDefaultAsync(s => s.Id == viewModel.Id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {viewModel.Id} not found.");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                _httpContextAccessor.HttpContext.User,
                subject,
                "SubjectCanEdit");
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedAccessException($"User lacks permission to edit Subject with ID {viewModel.Id}.");
            }

            viewModel.Permissions |= SubjectPermissions.OwnerCanView |
                                     SubjectPermissions.AdminCanView |
                                     SubjectPermissions.AdminCanEdit |
                                     SubjectPermissions.AdminCanDelete;

            // Map view model properties to the subject entity
            _mapper.Map(viewModel, subject);

            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            // Get the IDs of tags the user can edit (UserId matches current user or is null, unless admin)
            var editableTagIds = await _context.MetaTags
                .Where(m => isAdmin || m.UserId == currentUserId || m.UserId == null)
                .Select(m => m.Id)
                .ToListAsync();

            // Remove editable tags from the subject
            var tagsToRemove = subject.MetaTags.Where(mt => editableTagIds.Contains(mt.Id)).ToList();
            foreach (var tag in tagsToRemove)
            {
                subject.MetaTags.Remove(tag);
            }

            // Add the user-selected tags (only those they are allowed to edit)
            var selectedTags = await _context.MetaTags
                .Where(m => viewModel.SelectedMetaTagIds.Contains(m.Id) && editableTagIds.Contains(m.Id))
                .ToListAsync();
            foreach (var tag in selectedTags)
            {
                subject.MetaTags.Add(tag);
            }

            _context.Update(subject);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }

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