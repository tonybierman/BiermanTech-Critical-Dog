using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BiermanTech.CriticalDog.Services
{
    public class SubjectObservationService : ISubjectObservationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubjectObservationService> _logger;

        public SubjectObservationService(AppDbContext context, ILogger<SubjectObservationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.GetFilteredSubjects()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ObservationDefinition?> GetObservationDefinitionByIdAsync(int? observationDefinitionId)
        {
            if (!observationDefinitionId.HasValue)
            {
                return null;
            }
            return await _context.ObservationDefinitions
                .Include(a => a.MetricTypes)
                .FirstOrDefaultAsync(od => od.Id == observationDefinitionId);
        }

        public async Task<MetricType?> GetMetricTypeByIdAsync(int? metricTypeId)
        {
            if (!metricTypeId.HasValue)
            {
                return null;
            }
            return await _context.MetricTypes
                .FirstOrDefaultAsync(mt => mt.Id == metricTypeId);
        }

        public async Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null)
        {
            var items = await _context.ObservationDefinitions
                .Where(od => od.IsActive == true)
                .Select(od => new SelectListItem
                {
                    Value = od.Id.ToString(),
                    Text = od.DefinitionName
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedId?.ToString());
        }

        public async Task<SelectList> GetMetricTypesSelectListAsync(int observationDefinitionId, int? selectedId = null)
        {
            var items = await _context.MetricTypes
                .Where(mt => mt.ObservationDefinitionId == observationDefinitionId && mt.IsActive == true)
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Description
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedId?.ToString());
        }

        public async Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null)
        {
            var items = await _context.MetaTags
                .Where(mt => mt.IsActive == true)
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.TagName
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedIds);
        }

        public async Task SaveSubjectRecordAsync(SubjectRecord record, IEnumerable<int>? selectedMetaTagIds)
        {
            try
            {
                _context.Add(record); // Use generic Add; UserId set by ApplyUserIdOnSave
                await _context.SaveChangesAsync();

                if (selectedMetaTagIds?.Any() == true)
                {
                    foreach (var tagId in selectedMetaTagIds)
                    {
                        var metaTag = await _context.MetaTags.FirstOrDefaultAsync(mt => mt.Id == tagId);
                        if (metaTag != null)
                        {
                            record.MetaTags.Add(metaTag);
                        }
                        else
                        {
                            _logger.LogWarning("MetaTag with Id {TagId} not found.", tagId);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SubjectRecord with MetaTagIds {MetaTagIds}", selectedMetaTagIds);
                throw;
            }
        }
    }
}