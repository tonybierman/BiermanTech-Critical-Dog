using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2
{ 
    public class CreateStep2PostContext 
    { 
        public int DogId { get; set; } 
        public CreateObservationViewModel ObservationVM { get; set; } 
        public ObservationDefinition ObservationDefinition { get; set; }
        public IEnumerable<SelectListItem> SelectedListItems { get; set; } 
        public int SelectedItem { get; set; } 
        public IActionResult Result { get; set; } 
        public PageModel PageModel { get; set; } 
    } 
}