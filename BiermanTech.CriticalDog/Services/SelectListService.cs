using AutoMapper;
using BiermanTech.CriticalDog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class SelectListService : ISelectListService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SelectListService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SelectList> GetMetricTypesSelectListAsync(IEnumerable<int>? selectedIds = null)
        {
            var items = await _context.MetricTypes
                .Where(mt => mt.IsActive == true)
                .OrderBy(mt => mt.Description)
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Description
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text", selectedIds);
        }

        public async Task<SelectList> GetObservationTypesSelectListAsync()
        {
            var types = await _context.ObservationTypes.ToListAsync();
            return new SelectList(types, nameof(ObservationType.Id), nameof(ObservationType.TypeName));
        }

        public async Task<SelectList> GetScientificDisciplinesSelectListAsync()
        {
            var disciplines = await _context.ScientificDisciplines.ToListAsync();
            return new SelectList(disciplines, nameof(ScientificDiscipline.Id), nameof(ScientificDiscipline.DisciplineName));
        }
    }
}
