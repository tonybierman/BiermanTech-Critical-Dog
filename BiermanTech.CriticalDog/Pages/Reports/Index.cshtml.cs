using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UniversalReportCore.Helpers;
using UniversalReportCore.PageMetadata;
using UniversalReportCore.ViewModels;
using UniversalReportCore;
using BiermanTech.CriticalDog.Reports;
using UniversalReportCore.Ui.Pages;
using UniversalReportCore.HardQuerystringVariables;
using Microsoft.CSharp.RuntimeBinder;
using UniversalReportCore.Ui;
using UniversalReportCore.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BiermanTech.CriticalDog.Pages.Reports
{
    public class IndexModel : ReportPageModel
    {
        public IndexModel(ILogger<IndexModel> logger,
            IMapper mapper,
            IReportColumnFactory reportColumnFactory,
            IPageMetaFactory pageMetaFactory,
            IReportPageHelperFactory pageHelperFactory,
            IAuthorizationService authorizationService,
            IAuthorizationPolicyProvider policyProvider) :
            base(logger, mapper, pageMetaFactory,
                reportColumnFactory, pageHelperFactory, 
                authorizationService, policyProvider)
        {
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await ReportPageGetAsync();

            return result;
        }
    }
}
