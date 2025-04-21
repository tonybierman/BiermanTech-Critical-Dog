using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BiermanTech.CriticalDog
{
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
                Text = mt.Description // Assuming MetricType has a Name property
            });

            return new SelectList(items, "Value", "Text");
        }
    }

    public class DefaultMappingProfile : Profile
    {
        public DefaultMappingProfile()
        {
            CreateMap<SubjectRecord, SubjectRecordViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubjectRecord, SubjectRecordInputViewModel>()
                .ForMember(dest => dest.SelectedMetaTagIds, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubjectRecordInputViewModel, SubjectRecord>()
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationDefinition, opt => opt.Ignore())
                .ForMember(dest => dest.MetricType, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTags, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<SubjectType, SubjectTypeInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<SubjectTypeInputViewModel, SubjectType>()
                .ForMember(dest => dest.Subjects, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<ObservationType, ObservationTypeInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<ObservationTypeInputViewModel, ObservationType>()
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MetricType, MetricTypeInputViewModel>();
            CreateMap<MetricTypeInputViewModel, MetricType>()
                .ForMember(dest => dest.ObservationDefinition, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MetaTag, MetaTagInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MetaTagInputViewModel, MetaTag>()
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Unit, UnitInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UnitInputViewModel, Unit>()
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<ObservationDefinition, ObservationDefinitionInputViewModel>()
                .ForMember(dest => dest.SelectedScientificDisciplineIds, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ObservationDefinitionInputViewModel, ObservationDefinition>()
                .ForMember(dest => dest.ScientificDisciplines, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationType, opt => opt.Ignore())
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForMember(dest => dest.Units, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<ScientificDiscipline, ScientificDisciplineInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ScientificDisciplineInputViewModel, ScientificDiscipline>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Subject, SubjectInputViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubjectInputViewModel, Subject>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubjectRecord, SubjectRecordViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ICollection<MetricType>, SelectList>()
                .ConvertUsing<MetricTypesConverter>();

            CreateMap<ObservationDefinition, CreateObservationViewModel>()
                .ForMember(dest => dest.ObservationDefinitionId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.MetricTypes,
                    opt => opt.MapFrom(src => src.MetricTypes))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}