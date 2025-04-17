using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using UniversalReport.Services;
using UniversalReportCore;
using UniversalReportCore.PagedQueries;
using UniversalReportCore.Ui;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;

namespace BiermanTech.CriticalDog.Reports.Kennel
{
    public class KennelPageHelper : PageHelperBase<SubjectRecord, SubjectRecordViewModel>
    {
        private readonly AppDbContext _dbContext;

        public KennelPageHelper(
            IUniversalReportService reportService,
            IReportColumnFactory reportColumnFactory,
            IQueryFactory<SubjectRecord> queryFactory,
            AppDbContext dbContext,
            IFilterProvider<SubjectRecord> filterProvider,
            FilterFactory<SubjectRecord> filterFactory,
            IMapper mapper) : base(reportService, reportColumnFactory, queryFactory, filterProvider, filterFactory, mapper)
        {
            _dbContext = dbContext;
            DefaultSort = "CityAsc";
        }

        public override async Task<PaginatedList<SubjectRecordViewModel>> GetPagedDataAsync(
            PagedQueryParameters<SubjectRecord> parameters,
            int totalCount)
        {
            IQueryable<SubjectRecord> query = _dbContext.GetFilteredSubjectRecords()
                .Include(a => a.Subject)
                .ThenInclude(a => a.SubjectType)
                .Include(a => a.ObservationDefinition)
                .ThenInclude(od => od.ObservationType)
                .Include(a => a.MetricType)
                .ThenInclude(mt => mt.Unit)
                .Include(a => a.MetaTags);

            return await _reportService.GetPagedAsync<SubjectRecord, SubjectRecordViewModel>(
                parameters, totalCount, query);
        }

        //public override async Task<ICohort[]?> GetCohortsAsync(int[] cohortIds)
        //{
        //    var cohorts = await _dbContext.SubjectRecordCohorts
        //                    .Where(c => cohortIds.Contains(c.Id))
        //                    .Cast<ICohort>() // Ensure conversion to ICohort
        //                    .ToArrayAsync();

        //    return cohorts.Length > 0 ? cohorts : null;
        //}

        public override List<IReportColumnDefinition> GetReportColumns(string slug)
        {
            var columns = _reportColumnFactory.GetReportColumns(slug);

            // Find the first DefaultSort value from sortable columns with a non-null DefaultSort property
            var defaultSortColumn = columns.FirstOrDefault(c => c.IsSortable && c.DefaultSort != null);
            if (defaultSortColumn != null)
            {
                DefaultSort = $"{defaultSortColumn.PropertyName}{defaultSortColumn.DefaultSort}";
            }

            return columns;
        }

        public override PagedQueryParameters<SubjectRecord> CreateQueryParameters(string queryType, IReportColumnDefinition[] columns, int? pageIndex, string? sort, int? ipp, int[]? cohortIds)
        {
            return _queryFactory.CreateQueryParameters(queryType, columns, pageIndex, sort, ipp, cohortIds);
        }
    }
}
