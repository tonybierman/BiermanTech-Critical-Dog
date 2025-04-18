using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class Step2GetContext
    {
        public int DogId { get; set; }
        public int? ObservationDefinitionId { get; set; }
        public CreateObservationViewModel ObservationVM { get; set; }
        public ObservationDefinition ObservationDefinition { get; set; }
        public Subject Dog { get; set; }
        public IActionResult Result { get; set; }
        public PageModel PageModel { get; set; }
        public string ObservationDefinitionName { get; set; }
    }
}