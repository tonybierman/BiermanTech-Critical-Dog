using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Services
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

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            _logger.LogInformation($"GetSubjectByIdAsync: Retrieving Subject with ID {id}.");
            var subject = await _context.GetFilteredSubjects()
                .Include(s => s.SubjectType)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null)
            {
                _logger.LogWarning($"GetSubjectByIdAsync: Subject with ID {id} not found or user lacks view permissions.");
            }
            else
            {
                _logger.LogInformation($"GetSubjectByIdAsync: Retrieved Subject ID={id}, Permissions={subject.Permissions}, UserId={subject.UserId}.");
            }
            return subject;
        }

        public async Task<SubjectInputViewModel> GetSubjectViewModelByIdAsync(int id)
        {
            var subject = await GetSubjectByIdAsync(id);
            return subject == null ? null : _mapper.Map<SubjectInputViewModel>(subject);
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
            // UserId and default Permissions are set in AppDbContext.ApplyUserIdOnSave

            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateSubjectAsync(SubjectInputViewModel viewModel)
        {
            var subject = await _context.Subjects
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

            _mapper.Map(viewModel, subject);
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