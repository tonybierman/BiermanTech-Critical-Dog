using Microsoft.AspNetCore.Razor.TagHelpers;
using UniversalReportCore;
using UniversalReportCore.Ui;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Reports
{
    public class PageHelperFactory : ReportPageHelperFactoryBase, IReportPageHelperFactory
    {
        public PageHelperFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _helperMap = new Dictionary<string, (Type, Type)>
            {
                { "SubjectRecordReports", (typeof(SubjectRecord), typeof(SubjectRecordViewModel)) }
            };
        }
    }
}
