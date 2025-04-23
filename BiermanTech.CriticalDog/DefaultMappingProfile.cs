using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BiermanTech.CriticalDog
{
    public class DefaultMappingProfile : Profile
    {
        public DefaultMappingProfile()
        {
            // ICollection<MetricType>
            CreateMap<ICollection<MetricType>, SelectList>()
                .ConvertUsing<MetricTypesConverter>();

            // MetaTag
            CreateMap<MetaTag, MetaTagInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // MetaTagInputViewModel
            CreateMap<MetaTagInputViewModel, MetaTag>()
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // MetricType
            CreateMap<MetricType, MetricTypeInputViewModel>()
                .ForMember(dest => dest.ObservationDefinitionIds,
                    opt => opt.MapFrom(src => src.ObservationDefinitions.Select(od => od.Id).ToList()));

            // MetricTypeInputViewModel
            CreateMap<MetricTypeInputViewModel, MetricType>()
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ObservationDefinition
            CreateMap<ObservationDefinition, ObservationDefinitionInputViewModel>()
                .ForMember(dest => dest.SelectedScientificDisciplineIds,
                    opt => opt.MapFrom(src => src.ScientificDisciplines.Select(d => d.Id).ToList()))
                .ForMember(dest => dest.MetricTypeIds,
                    opt => opt.MapFrom(src => src.MetricTypes.Select(d => d.Id).ToList()));

            CreateMap<ObservationDefinition, CreateObservationViewModel>()
                .ForMember(dest => dest.ObservationDefinitionId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MetricTypes,
                    opt => opt.MapFrom(src => src.MetricTypes))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ObservationDefinitionInputViewModel
            CreateMap<ObservationDefinitionInputViewModel, ObservationDefinition>()
                .ForMember(dest => dest.ScientificDisciplines, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationType, opt => opt.Ignore())
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForMember(dest => dest.Units, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ObservationType
            CreateMap<ObservationType, ObservationTypeInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ObservationTypeInputViewModel
            CreateMap<ObservationTypeInputViewModel, ObservationType>()
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ScientificDiscipline
            CreateMap<ScientificDiscipline, ScientificDisciplineInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ScientificDisciplineInputViewModel
            CreateMap<ScientificDisciplineInputViewModel, ScientificDiscipline>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Subject
            CreateMap<Subject, SubjectViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Subject, SubjectInputViewModel>()
                .ForMember(dest => dest.SelectedMetaTagIds,
                    opt => opt.MapFrom(src => src.MetaTags.Select(d => d.Id).ToList()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SubjectInputViewModel
            CreateMap<SubjectInputViewModel, Subject>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SubjectRecord
            CreateMap<SubjectRecord, SubjectRecordViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubjectRecord, SubjectRecordInputViewModel>()
                .ForMember(dest => dest.SelectedMetaTagIds,
                    opt => opt.MapFrom(src => src.MetaTags.Select(d => d.Id).ToList()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SubjectRecordInputViewModel
            CreateMap<SubjectRecordInputViewModel, SubjectRecord>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationDefinition, opt => opt.Ignore())
                .ForMember(dest => dest.MetricType, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTags, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SubjectType
            CreateMap<SubjectType, SubjectTypeInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SubjectTypeInputViewModel
            CreateMap<SubjectTypeInputViewModel, SubjectType>()
                .ForMember(dest => dest.Subjects, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Unit
            CreateMap<Unit, UnitInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UnitInputViewModel
            CreateMap<UnitInputViewModel, Unit>()
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class MetricTypesConverter : ITypeConverter<ICollection<MetricType>, SelectList>
    {
        public SelectList Convert(ICollection<MetricType> source, SelectList destination, ResolutionContext context)
        {
            if (source == null || !source.Any())
            {
                return new SelectList(Enumerable.Empty<SelectListItem>());
            }

            var items = source.Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(), // Assuming MetricType has an Id property
                Text = mt.Name // Assuming MetricType has a Name property
            });

            return new SelectList(items, "Value", "Text");
        }
    }
}