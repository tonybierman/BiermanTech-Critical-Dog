using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services
{
    public class SubjectRecordService : ISubjectRecordService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SubjectRecordService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SubjectRecord> GetMostRecentSubjectRecordAsync(int subjectId, string definitionName)
        {
            return await _context.GetFilteredSubjectRecords()
                .Include(s => s.Subject)
                .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                .Include(s => s.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(s => s.MetaTags)
                .Where(s => s.SubjectId == subjectId && s.ObservationDefinition.Name == definitionName)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<SubjectRecord> GetSubjectRecordByIdAsync(int id)
        {
            return await _context.GetFilteredSubjectRecords()
                .Include(s => s.Subject)
                .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                .Include(s => s.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(s => s.MetaTags)
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
            viewModel.SelectedMetaTagIds = record.MetaTags.Select(m => m.Id).ToList();
            return viewModel;
        }

        public async Task<List<SubjectRecordInputViewModel>> GetAllSubjectRecordsAsync()
        {
            var records = await _context.GetFilteredSubjectRecords()
                .Include(s => s.MetaTags)
                .Include(s => s.Subject)
                .ThenInclude(s => s.SubjectType)
                .Include(s => s.ObservationDefinition)
                .Include(s => s.MetricType)
                .ThenInclude(mt => mt.Unit)
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

            _mapper.Map(viewModel, record);
            record.MetaTags.Clear();
            record.MetaTags = await _context.MetaTags
                .Where(m => viewModel.SelectedMetaTagIds.Contains(m.Id))
                .ToListAsync();
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