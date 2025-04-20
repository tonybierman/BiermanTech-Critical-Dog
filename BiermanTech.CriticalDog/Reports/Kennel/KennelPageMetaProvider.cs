using UniversalReportCore;
using UniversalReportCore.PageMetadata;
using UniversalReportCore.ViewModels;
using BiermanTech.CriticalDog.Reports;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    //[PageMetaPolicy("RequireAuthenticated")]
    public class KennelPageMetaProvider : BaseReportPageMetaProvider, IPageMetaProvider 
    {
        public string Slug => "Kennel";

        public override string CategorySlug => "SubjectRecordReports";

        public PageMetaViewModel GetPageMeta()
        {
            return new PageMetaViewModel() { Title = "My Dogs", Subtitle = "Kennel Report" };
        }
    }
}
