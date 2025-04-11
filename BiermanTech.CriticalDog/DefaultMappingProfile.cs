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