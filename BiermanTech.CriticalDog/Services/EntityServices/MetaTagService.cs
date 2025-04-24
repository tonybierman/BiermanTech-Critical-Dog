using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class MetaTagService : IMetaTagService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MetaTagService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MetaTag> GetMetaTagByIdAsync(int id)
        {
            return await _context.MetaTags
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MetaTagInputViewModel> GetMetaTagViewModelByIdAsync(int id)
        {
            var metaTag = await GetMetaTagByIdAsync(id);
            return metaTag == null ? null : _mapper.Map<MetaTagInputViewModel>(metaTag);
        }

        public async Task<List<MetaTagInputViewModel>> GetAllMetaTagsAsync()
        {
            var metaTags = await _context.MetaTags.ToListAsync();
            return _mapper.Map<List<MetaTagInputViewModel>>(metaTags);
        }

        public async Task CreateMetaTagAsync(MetaTagInputViewModel viewModel)
        {
            var entity = _mapper.Map<MetaTag>(viewModel);
            _context.MetaTags.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMetaTagAsync(MetaTagInputViewModel viewModel)
        {
            var metaTag = await _context.MetaTags.FindAsync(viewModel.Id);
            if (metaTag == null)
            {
                throw new KeyNotFoundException($"MetaTag with ID {viewModel.Id} not found.");
            }

            _mapper.Map(viewModel, metaTag);
            _context.MetaTags.Update(metaTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMetaTagAsync(int id)
        {
            var metaTag = await _context.MetaTags.FindAsync(id);
            if (metaTag == null)
            {
                throw new KeyNotFoundException($"MetaTag with ID {id} not found.");
            }

            _context.MetaTags.Remove(metaTag);
            await _context.SaveChangesAsync();
        }
    }
}