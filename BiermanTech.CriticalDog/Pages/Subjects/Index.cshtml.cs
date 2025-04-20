using AutoMapper;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Subjects;

namespace BiermanTech.CriticalDog.Pages.Dogs
{
    public class IndexModel : SubjectBasePageModel
    {
        public IndexModel(ISubjectService subjectService, IMapper mapper, IAuthorizationService authorizationService, ILogger<IndexModel> logger) : 
            base(subjectService, mapper, authorizationService, logger) { }

        public IList<Subject> Dogs { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            _logger.LogInformation($"IndexModel.OnGetAsync: Retrieving filtered subjects for user: {User.Identity.Name}, IsAdmin: {User.IsInRole("Admin")}");

            Dogs = await _subjectService.GetFilteredSubjectsAsync();

            _logger.LogInformation($"IndexModel.OnGetAsync: Retrieved {Dogs.Count} subjects.");
        }
    }

}