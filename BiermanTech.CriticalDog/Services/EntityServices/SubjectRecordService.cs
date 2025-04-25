using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class SubjectRecordService : ISubjectRecordService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubjectRecordService(AppDbContext context, IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<SubjectRecord>> GetMostRecentSubjectRecordsByDisciplineAsync(int subjectId, string? scientificDisciplineNameFilter = null)
        {
            try
            {
                // Get the current user's ID and admin status
                var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

                var query = _context.GetFilteredSubjectRecords()
                    .Include(s => s.Subject)
                        .ThenInclude(s => s.SubjectType)
                    .Include(s => s.ObservationDefinition)
                        .ThenInclude(od => od.ObservationType)
                    .Include(s => s.ObservationDefinition)
                        .ThenInclude(od => od.ScientificDisciplines)
                    .Include(s => s.MetricType)
                        .ThenInclude(mt => mt.Unit)
                    .Include(s => s.MetaTags
                        .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                    .Where(s => s.SubjectId == subjectId)
                    .AsNoTracking();

                // Apply optional filter for scientificDisciplineNameFilter
                if (!string.IsNullOrWhiteSpace(scientificDisciplineNameFilter))
                {
                    var filterLower = scientificDisciplineNameFilter.ToLower();
                    query = query.Where(s => s.ObservationDefinition.ScientificDisciplines
                        .Any(sd => sd.Name.ToLower().Contains(filterLower)));
                }

                // Use a subquery to get the most recent record per ObservationDefinition.Name
                var records = await query
                    .Where(sr => sr.Id == _context.GetFilteredSubjectRecords()
                        .Where(inner => inner.SubjectId == sr.SubjectId
                                     && inner.ObservationDefinition.Name == sr.ObservationDefinition.Name)
                        .OrderByDescending(inner => inner.CreatedAt)
                        .Select(inner => inner.Id)
                        .FirstOrDefault())
                    .ToListAsync();

                return records;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve subject records for subjectId {subjectId}.", ex);
            }
        }

        public async Task<IEnumerable<SubjectRecord>> GetMostRecentSubjectRecordsAsync(int subjectId)
        {
            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var records = await _context.GetFilteredSubjectRecords()
                .Include(s => s.Subject)
                    .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ObservationType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ScientificDisciplines)
                .Include(s => s.MetricType)
                    .ThenInclude(mt => mt.Unit)
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .Where(s => s.SubjectId == subjectId)
                .GroupBy(s => s.ObservationDefinition.Name)
                .Select(g => g.OrderByDescending(s => s.CreatedAt).First())
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName)
        {
            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            return await _context.GetFilteredSubjectRecords()
                .Include(s => s.Subject)
                    .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ObservationType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ScientificDisciplines)
                .Include(s => s.MetricType)
                    .ThenInclude(mt => mt.Unit)
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .Where(s => s.SubjectId == subjectId && s.ObservationDefinition.Name == definitionName)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SubjectRecord> GetSubjectRecordByIdAsync(int id)
        {
            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            return await _context.GetFilteredSubjectRecords()
                .Include(s => s.Subject)
                    .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ObservationType)
                .Include(s => s.ObservationDefinition)
                    .ThenInclude(od => od.ScientificDisciplines)
                .Include(s => s.MetricType)
                    .ThenInclude(mt => mt.Unit)
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SubjectRecordInputViewModel> GetSubjectRecordViewModelByIdAsync(int id)
        {
            var record = await GetSubjectRecordByIdAsync(id);
            if (record == null)
            {
                return null;
            }
            var viewModel = _mapper.Map<SubjectRecordInputViewModel>(record);

            return viewModel;
        }

        public async Task<List<SubjectRecordInputViewModel>> GetAllSubjectRecordsAsync()
        {
            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            var records = await _context.GetFilteredSubjectRecords()
                .Include(s => s.MetaTags
                    .Where(mt => isAdmin || mt.UserId == currentUserId || mt.UserId == null))
                .Include(s => s.Subject)
                    .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                .Include(s => s.MetricType)
                    .ThenInclude(mt => mt.Unit)
                .AsNoTracking()
                .ToListAsync();

            var viewModels = _mapper.Map<List<SubjectRecordInputViewModel>>(records);

            for (int i = 0; i < records.Count; i++)
            {
                viewModels[i].SelectedMetaTagIds = records[i].MetaTags.Select(m => m.Id).ToList();
            }

            return viewModels;
        }

        public async Task CreateSubjectRecordAsync(SubjectRecordInputViewModel viewModel)
        {
            var entity = _mapper.Map<SubjectRecord>(viewModel);
            entity.MetaTags = await _context.MetaTags
                .Where(m => viewModel.SelectedMetaTagIds.Contains(m.Id))
                .ToListAsync();
            _context.Add(entity); // EF Core allows adding entities directly
            await _context.SaveChangesAsync(); // ApplyUserIdOnSave sets Subject.UserId
        }

        public async Task UpdateSubjectRecordAsync(SubjectRecordInputViewModel viewModel)
        {
            var record = await _context.GetFilteredSubjectRecords()
                .Include(s => s.MetaTags)
                .FirstOrDefaultAsync(s => s.Id == viewModel.Id);
            if (record == null)
            {
                throw new KeyNotFoundException($"SubjectRecord with ID {viewModel.Id} not found or you lack permission to access it.");
            }

            // Get the current user's ID and admin status
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            // Map view model properties to the record entity
            _mapper.Map(viewModel, record);

            // Get the IDs of tags the user can edit (UserId matches current user or is null, unless admin)
            var editableTagIds = await _context.MetaTags
                .Where(m => isAdmin || m.UserId == currentUserId || m.UserId == null)
                .Select(m => m.Id)
                .ToListAsync();

            // Remove editable tags from the record
            var tagsToRemove = record.MetaTags.Where(mt => editableTagIds.Contains(mt.Id)).ToList();
            foreach (var tag in tagsToRemove)
            {
                record.MetaTags.Remove(tag);
            }

            // Add the user-selected tags (only those they are allowed to edit)
            var selectedTags = await _context.MetaTags
                .Where(m => viewModel.SelectedMetaTagIds.Contains(m.Id) && editableTagIds.Contains(m.Id))
                .ToListAsync();
            foreach (var tag in selectedTags)
            {
                record.MetaTags.Add(tag);
            }

            _context.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubjectRecordAsync(int id)
        {
            var record = await _context.GetFilteredSubjectRecords()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (record == null)
            {
                throw new KeyNotFoundException($"SubjectRecord with ID {id} not found or you lack permission to access it.");
            }

            _context.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}