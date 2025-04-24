using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Pages.Subjects;
using BiermanTech.CriticalDog.ViewModels;
using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.Pages.Dogs
{
    public class IndexModel : SubjectBasePageModel
    {
        public Dictionary<int, List<string>> MetaTagNames { get; set; } = new Dictionary<int, List<string>>();

        public IndexModel(ISubjectService subjectService, IMapper mapper, IAuthorizationService authorizationService, ILogger<IndexModel> logger) : 
            base(subjectService, mapper, authorizationService, logger) { }

        public IList<SubjectViewModel> Subjects { get; set; } = new List<SubjectViewModel>();

        public async Task OnGetAsync()
        {
            Subjects = await _subjectService.GetFilteredSubjectViewModelsAsync();
            foreach (var item in Subjects)
            {
                MetaTagNames[item.Id] = item.SelectedMetaTagNames;
            }
        }
    }

}