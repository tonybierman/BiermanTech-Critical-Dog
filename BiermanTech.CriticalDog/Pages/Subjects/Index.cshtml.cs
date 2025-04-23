using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Subjects;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Pages.Dogs
{
    public class IndexModel : SubjectBasePageModel
    {
        public IndexModel(ISubjectService subjectService, IMapper mapper, IAuthorizationService authorizationService, ILogger<IndexModel> logger) : 
            base(subjectService, mapper, authorizationService, logger) { }

        public IList<SubjectViewModel> Dogs { get; set; } = new List<SubjectViewModel>();

        public async Task OnGetAsync()
        {
            _logger.LogInformation($"IndexModel.OnGetAsync: Retrieving filtered subjects for user: {User.Identity.Name}, IsAdmin: {User.IsInRole("Admin")}");

            Dogs = await _subjectService.GetFilteredSubjectViewModelsAsync();

            _logger.LogInformation($"IndexModel.OnGetAsync: Retrieved {Dogs.Count} subjects.");
        }
    }

}