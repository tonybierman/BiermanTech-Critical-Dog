using AutoMapper;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Models;
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
            CreateMap<MetaTag, MetaTagInputViewModel>();
            CreateMap<MetaTagInputViewModel, MetaTag>()
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore());

            CreateMap<Unit, UnitInputViewModel>();
            CreateMap<UnitInputViewModel, Unit>()
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationDefinitions, opt => opt.Ignore());

            CreateMap<ObservationDefinition, ObservationDefinitionInputViewModel>()
                .ForMember(dest => dest.SelectedScientificDisciplineIds, opt => opt.Ignore());
            CreateMap<ObservationDefinitionInputViewModel, ObservationDefinition>()
                .ForMember(dest => dest.ScientificDisciplines, opt => opt.Ignore())
                .ForMember(dest => dest.ObservationType, opt => opt.Ignore())
                .ForMember(dest => dest.MetricTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectRecords, opt => opt.Ignore())
                .ForMember(dest => dest.Units, opt => opt.Ignore());

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
                .ForMember( dest => dest.ObservationDefinitionId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.MetricTypes,
                    opt => opt.MapFrom(src => src.MetricTypes))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}