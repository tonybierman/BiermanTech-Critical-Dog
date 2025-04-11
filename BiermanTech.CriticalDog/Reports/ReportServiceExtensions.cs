using Microsoft.Extensions.DependencyInjection;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports.CityPop;
using BiermanTech.CriticalDog.ViewModels;
using UniversalReport.Services;
using AutoMapper;
using UniversalReportCore.PagedQueries;
using UniversalReportCore.PageMetadata;
using UniversalReportCore.Ui;
using UniversalReportCore;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using BiermanTech.CriticalDog.Reports.Domain;
using BiermanTech.CriticalDog.Reports.Kennel;

namespace BiermanTech.CriticalDog.Reports
{
    public static class ReportServiceExtensions
    {
        /// <summary>
        /// Registers CityPopDemo report.
        /// </summary>
        public static IServiceCollection AddCityPopDemoReport(this IServiceCollection services) =>
            services.AddEntityReportServices<
                DogRecord,
                DogRecordViewModel,
                DogRecordQueryFactory,
                KennelPageMetaProvider,
                KennelReportColumnProvider,
                KennelQueryProvider,
                KennelPageHelper,
                DogRecordFilterProvider>();

        public static IServiceCollection AddUniversalReportServices(this IServiceCollection services)
        {
            // Add Universal Report Core services
            services.AddUniversalReport();

            // Register the IUniversalReportService here
            services.AddScoped<IUniversalReportService>(provider =>
            {
                var dbContext = provider.GetRequiredService<AppDbContext>();
                var mapper = provider.GetRequiredService<IMapper>();
                return new UniversalReportService(dbContext, mapper);
            });

            services.AddScoped<IReportPageHelperFactory, PageHelperFactory>();
            services.AddCityPopDemoReport();

            return services;
        }
    }
}
