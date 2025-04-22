using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class SelectListService : ISelectListService
    {
        private readonly AppDbContext _context;

        public SelectListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SelectList> GetMetricTypesSelectListAsync(int? observationDefinitionId = null, int? selectedId = null, IEnumerable<int>? selectedIds = null, bool filterActive = true)
        {
            IQueryable<MetricType> query = _context.MetricTypes.AsQueryable();

            if (filterActive)
            {
                query = query.Where(mt => mt.IsActive == true);
            }

            if (observationDefinitionId.HasValue)
            {
                var metricTypeIds = await _context.Set<Dictionary<string, object>>("ObservationDefinitionMetricType")
                    .Where(j => (int)j["ObservationDefinitionId"] == observationDefinitionId.Value)
                    .Select(j => (int)j["MetricTypeId"])
                    .ToListAsync();

                query = query.Where(mt => metricTypeIds.Contains(mt.Id));
            }

            var items = await query
                .OrderBy(mt => mt.Description)
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Description
                })
                .ToListAsync();

            // Use selectedIds for multi-select, otherwise fall back to selectedId for single-select
            object selectedValue = selectedIds != null ? selectedIds.Select(id => id.ToString()) : (object)selectedId?.ToString();

            return new SelectList(items, "Value", "Text", selectedValue);
        }

        public async Task<SelectList> GetMetaTagsSelectListAsync(IEnumerable<int>? selectedIds = null, bool filterActive = true)
        {
            var query = _context.MetaTags.AsQueryable();

            if (filterActive)
            {
                query = query.Where(mt => mt.IsActive == true);
            }

            query = query.OrderBy(mt => mt.TagName);

            if (selectedIds != null)
            {
                var items = await query
                    .Select(mt => new SelectListItem
                    {
                        Value = mt.Id.ToString(),
                        Text = StringHelper.SplitPascalCase(mt.TagName)
                    })
                    .ToListAsync();

                return new SelectList(items, "Value", "Text", selectedIds.Select(id => id.ToString()));
            }
            else
            {
                var metaTags = await query.ToListAsync();
                return new SelectList(metaTags, nameof(MetaTag.Id), nameof(MetaTag.TagName));
            }
        }

        public async Task<SelectList> GetSubjectTypesSelectListAsync()
        {
            var subjectTypes = await _context.SubjectTypes
                .OrderBy(st => st.TypeName)
                .Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = StringHelper.SplitPascalCase(st.TypeName)
                })
                .ToListAsync();

            return new SelectList(subjectTypes, "Value", "Text");
        }

        public async Task<SelectList> GetSubjectsSelectListAsync()
        {
            var subjects = await _context.GetFilteredSubjects()
                .OrderBy(s => s.Name)
                .ToListAsync();

            return new SelectList(subjects, nameof(Subject.Id), nameof(Subject.Name));
        }

        public async Task<SelectList> GetObservationDefinitionsSelectListAsync(int? selectedId = null, bool filterActive = true)
        {
            var query = _context.ObservationDefinitions.AsQueryable();

            if (filterActive)
            {
                query = query.Where(od => od.IsActive == true);
            }

            query = query.OrderBy(od => od.DefinitionName);

            var items = await query
                .Select(od => new SelectListItem
                {
                    Value = od.Id.ToString(),
                    Text = StringHelper.SplitPascalCase(od.DefinitionName)
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedId?.ToString());
        }

        public async Task<SelectList> GetUnitsSelectListAsync(bool filterActive = true)
        {
            var query = _context.Units.AsQueryable();

            if (filterActive)
            {
                query = query.Where(u => u.IsActive == true);
            }

            var units = await query
                .OrderBy(u => u.UnitName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = StringHelper.SplitPascalCase(u.UnitName)
                })
                .ToListAsync();

            return new SelectList(units, "Value", "Text");
        }

        public async Task<SelectList> GetObservationTypesSelectListAsync(bool filterActive = true)
        {
            var query = _context.ObservationTypes.AsQueryable();

            if (filterActive)
            {
                query = query.Where(ot => ot.IsActive == true);
            }

            var types = await query
                .OrderBy(ot => ot.TypeName)
                .Select(ot => new SelectListItem
                {
                    Value = ot.Id.ToString(),
                    Text = StringHelper.SplitPascalCase(ot.TypeName)
                })
                .ToListAsync();

            return new SelectList(types, "Value", "Text");
        }

        public async Task<SelectList> GetScientificDisciplinesSelectListAsync(bool filterActive = true)
        {
            var query = _context.ScientificDisciplines.AsQueryable();

            if (filterActive)
            {
                query = query.Where(sd => sd.IsActive == true);
            }

            var disciplines = await query
                .OrderBy(sd => sd.DisciplineName)
                .Select(sd => new SelectListItem
                {
                    Value = sd.Id.ToString(),
                    Text = StringHelper.SplitPascalCase(sd.DisciplineName)
                })
                .ToListAsync();

            return new SelectList(disciplines, "Value", "Text");
        }
    }
}