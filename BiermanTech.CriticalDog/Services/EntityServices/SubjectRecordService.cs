using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class SubjectRecordService : ISubjectRecordService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubjectRecordService(
            AppDbContext context,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Centralized user context retrieval
        private (string CurrentUserId, bool IsAdmin) GetUserContext()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return (user.FindFirst(ClaimTypes.NameIdentifier)?.Value, user.IsInRole("Admin"));
        }

        // Base query with common includes and MetaTags filtering
        private IQueryable<SubjectRecord> GetBaseSubjectRecordQuery(string currentUserId, bool isAdmin)
        {
            return _context.GetFilteredSubjectRecords()
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
                .AsNoTracking();
        }

        // Flexible query builder for SubjectRecord queries
        private IQueryable<SubjectRecord> BuildSubjectRecordQuery(
            string currentUserId,
            bool isAdmin,
            int? subjectId = null,
            string definitionName = null,
            string scientificDisciplineNameFilter = null)
        {
            var query = GetBaseSubjectRecordQuery(currentUserId, isAdmin);

            if (subjectId.HasValue)
            {
                query = query.Where(s => s.SubjectId == subjectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(definitionName))
            {
                query = query.Where(s => s.ObservationDefinition.Name == definitionName);
            }

            if (!string.IsNullOrWhiteSpace(scientificDisciplineNameFilter))
            {
                var filterLower = scientificDisciplineNameFilter.ToLower();
                query = query.Where(s => s.ObservationDefinition.ScientificDisciplines
                    .Any(sd => sd.Name.ToLower().Contains(filterLower)));
            }

            return query;
        }

        // Centralized mapping to SubjectRecordInputViewModel
        private async Task<SubjectRecordInputViewModel> MapToViewModelAsync(SubjectRecord record)
        {
            var viewModel = _mapper.Map<SubjectRecordInputViewModel>(record);
            viewModel.SelectedMetaTagIds = record.MetaTags?.Select(m => m.Id).ToList() ?? new List<int>();
            return viewModel;
        }

        // Centralized mapping for collections
        private async Task<List<SubjectRecordInputViewModel>> MapToViewModelsAsync(IEnumerable<SubjectRecord> records)
        {
            var viewModels = new List<SubjectRecordInputViewModel>();
            foreach (var record in records)
            {
                viewModels.Add(await MapToViewModelAsync(record));
            }
            return viewModels;
        }

        // Centralized MetaTags assignment
        private async Task AssignMetaTagsAsync(SubjectRecord entity, List<int> selectedMetaTagIds)
        {
            entity.MetaTags = await _context.MetaTags
                .Where(m => selectedMetaTagIds.Contains(m.Id))
                .ToListAsync();
        }

        // Consistent exception handling
        private void ThrowNotFoundException(int id)
        {
            throw new KeyNotFoundException($"SubjectRecord with ID {id} not found or you lack permission to access it.");
        }

        public async Task<IEnumerable<SubjectRecord>> GetMostRecentSubjectRecordsByDisciplineAsync(
            int subjectId, string? scientificDisciplineNameFilter = null)
        {
            try
            {
                var (currentUserId, isAdmin) = GetUserContext();
                var query = BuildSubjectRecordQuery(currentUserId, isAdmin, subjectId, scientificDisciplineNameFilter: scientificDisciplineNameFilter);

                // Get most recent record per ObservationDefinition.Name
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
            var (currentUserId, isAdmin) = GetUserContext();
            var records = await BuildSubjectRecordQuery(currentUserId, isAdmin, subjectId)
                .GroupBy(s => s.ObservationDefinition.Name)
                .Select(g => g.OrderByDescending(s => s.CreatedAt).First())
                .ToListAsync();

            return records;
        }

        public async Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName)
        {
            var (currentUserId, isAdmin) = GetUserContext();
            return await BuildSubjectRecordQuery(currentUserId, isAdmin, subjectId, definitionName)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<SubjectRecord> GetSubjectRecordByIdAsync(int id)
        {
            var (currentUserId, isAdmin) = GetUserContext();
            return await GetBaseSubjectRecordQuery(currentUserId, isAdmin)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SubjectRecordInputViewModel> GetSubjectRecordViewModelByIdAsync(int id)
        {
            var record = await GetSubjectRecordByIdAsync(id);
            return record == null ? null : await MapToViewModelAsync(record);
        }

        public async Task<List<SubjectRecordInputViewModel>> GetAllSubjectRecordsAsync()
        {
            var (currentUserId, isAdmin) = GetUserContext();
            var records = await GetBaseSubjectRecordQuery(currentUserId, isAdmin).ToListAsync();
            return await MapToViewModelsAsync(records);
        }

        public async Task CreateSubjectRecordAsync(SubjectRecordInputViewModel viewModel)
        {
            var entity = _mapper.Map<SubjectRecord>(viewModel);
            await AssignMetaTagsAsync(entity, viewModel.SelectedMetaTagIds);
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubjectRecordAsync(SubjectRecordInputViewModel viewModel)
        {
            var (currentUserId, isAdmin) = GetUserContext();
            var record = await _context.GetFilteredSubjectRecords()
                .Include(s => s.MetaTags)
                .FirstOrDefaultAsync(s => s.Id == viewModel.Id);

            if (record == null)
            {
                ThrowNotFoundException(viewModel.Id);
            }

            // Map view model properties to the record entity
            _mapper.Map(viewModel, record);

            // Get editable tag IDs
            var editableTagIds = await _context.MetaTags
                .Where(m => isAdmin || m.UserId == currentUserId || m.UserId == null)
                .Select(m => m.Id)
                .ToListAsync();

            // Remove editable tags
            var tagsToRemove = record.MetaTags.Where(mt => editableTagIds.Contains(mt.Id)).ToList();
            foreach (var tag in tagsToRemove)
            {
                record.MetaTags.Remove(tag);
            }

            // Add selected tags (only those editable)
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
                ThrowNotFoundException(id);
            }

            _context.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}