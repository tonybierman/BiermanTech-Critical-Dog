using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Services.EntityServices
{
    public class MetaTagService : IMetaTagService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MetaTagService(
            AppDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user?.Id;
        }

        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        private async Task<bool> CanAccessMetaTagAsync(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return false;

            if (await IsAdminAsync())
                return true;

            var metaTag = await _context.MetaTags.FindAsync(id);
            return metaTag != null && (metaTag.UserId == userId || metaTag.UserId == null);
        }

        public async Task<MetaTag> GetMetaTagByIdAsync(int id)
        {
            if (!await CanAccessMetaTagAsync(id))
                return null;

            return await _context.MetaTags.FindAsync(id);
        }

        public async Task<MetaTagInputViewModel> GetMetaTagViewModelByIdAsync(int id)
        {
            var metaTag = await GetMetaTagByIdAsync(id);
            return metaTag == null ? null : _mapper.Map<MetaTagInputViewModel>(metaTag);
        }

        public async Task<List<MetaTagInputViewModel>> GetAllMetaTagsAsync()
        {
            var metaTags = await _context.GetFilteredMetaTags().ToListAsync();
            return _mapper.Map<List<MetaTagInputViewModel>>(metaTags);
        }

        public async Task CreateMetaTagAsync(MetaTagInputViewModel viewModel)
        {
            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User must be authenticated to create a meta tag.");

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && viewModel.IsSystemScoped)
                throw new UnauthorizedAccessException("Only administrators can create system-scoped meta tags.");

            var entity = _mapper.Map<MetaTag>(viewModel, opts =>
                opts.Items["CurrentUserId"] = userId);

            if (viewModel.IsSystemScoped)
                entity.UserId = null;

            _context.MetaTags.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMetaTagAsync(MetaTagInputViewModel viewModel)
        {
            if (!await CanAccessMetaTagAsync(viewModel.Id))
                throw new KeyNotFoundException($"MetaTag with ID {viewModel.Id} not found or not accessible.");

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && viewModel.IsSystemScoped)
                throw new UnauthorizedAccessException("Only administrators can set meta tags as system-scoped.");

            var metaTag = await _context.MetaTags.FindAsync(viewModel.Id);
            if (metaTag == null)
                throw new KeyNotFoundException($"MetaTag with ID {viewModel.Id} not found.");

            var userId = await GetCurrentUserIdAsync();
            _mapper.Map(viewModel, metaTag, opts =>
                opts.Items["CurrentUserId"] = userId);

            _context.MetaTags.Update(metaTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMetaTagAsync(int id)
        {
            if (!await CanAccessMetaTagAsync(id))
                throw new KeyNotFoundException($"MetaTag with ID {id} not found or not accessible.");

            var metaTag = await _context.MetaTags.FindAsync(id);
            if (metaTag == null)
                throw new KeyNotFoundException($"MetaTag with ID {id} not found.");

            var isAdmin = await IsAdminAsync();
            if (!isAdmin && metaTag.UserId == null)
                throw new UnauthorizedAccessException("Only administrators can delete system-scoped meta tags.");

            _context.MetaTags.Remove(metaTag);
            await _context.SaveChangesAsync();
        }
    }
}