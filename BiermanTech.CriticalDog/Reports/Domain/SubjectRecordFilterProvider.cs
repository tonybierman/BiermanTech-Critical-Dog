using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Helpers;
using BiermanTech.CriticalDog.Reports.Columns;
using UniversalReportCore;

namespace BiermanTech.CriticalDog.Reports.Domain
{
    public class SubjectRecordFilterProvider : BaseFilterProvider<SubjectRecord>
    {
        private AppDbContext _dbContext;

        public SubjectRecordFilterProvider(AppDbContext dbContext) : base(new List<Facet<SubjectRecord>>())
        {
            _dbContext = dbContext;

            // Dynamically fetch Subject Genders
            var genders = _dbContext.GetFilteredSubjectRecords()
                .Select(p => p.Subject.Sex)
                .Distinct()
                .ToList();

            // Create Facet with FacetValues for individual gender
            var genderFacetValues = genders.Select(c =>
                new FacetValue<SubjectRecord>(
                    key: c.ToString(),
                    filter: p => p.Subject.Sex == c,
                    displayName: EnumHelper.GetEnumDisplayName((SexEnum)c)
                )).ToList();

            // Add dynamic gender Facet
            Facets.Add(new Facet<SubjectRecord>("Subject.Sex", genderFacetValues));

            // Dynamically fetch Subject Names
            var subjects = _dbContext.GetFilteredSubjectRecords()
                .Select(p => p.Subject.Name)
                .Distinct()
                .ToList();

            // Create Facet with FacetValues for individual subject name
            var subjectFacetValues = subjects.Select(c =>
                new FacetValue<SubjectRecord>(
                    key: c,
                    filter: p => p.Subject.Name == c,
                    displayName: c
                )).ToList();

            // Add dynamic subject name Facet
            Facets.Add(new Facet<SubjectRecord>("Subject.Name", subjectFacetValues));

            // Dynamically fetch Observation Definition names
            var observationDefinitionNames = _dbContext.GetFilteredSubjectRecords()
                .Select(p => p.ObservationDefinition.DefinitionName)
                .Distinct()
                .ToList();

            // Create Facet with FacetValues for Observation Definition names
            var observationDefinitionNameFacetValues = observationDefinitionNames.Select(c =>
                new FacetValue<SubjectRecord>(
                    key: c,
                    filter: p => p.ObservationDefinition.DefinitionName == c,
                    displayName: c
                )).ToList();

            // Add dynamic Observation Definition name Facet
            Facets.Add(new Facet<SubjectRecord>("ObservationDefinition.DefinitionName", observationDefinitionNameFacetValues));

            // Dynamically fetch subject type names
            var subjectTypeNames = _dbContext.GetFilteredSubjectRecords()
                .Select(p => p.Subject.SubjectType.TypeName)
                .Distinct()
                .ToList();

            // Create Facet with FacetValues for subject type names
            var subjectTypeNameFacetValues = subjectTypeNames.Select(c =>
                new FacetValue<SubjectRecord>(
                    key: c,
                    filter: p => p.Subject.SubjectType.TypeName == c,
                    displayName: c
                )).ToList();

            // Add dynamic subject type name Facet
            Facets.Add(new Facet<SubjectRecord>("Subject.SubjectType.TypeName", subjectTypeNameFacetValues));
        }
    }
}
