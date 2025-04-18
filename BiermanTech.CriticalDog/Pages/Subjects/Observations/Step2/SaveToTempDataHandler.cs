using System.Text.Json;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2
{ 
    public class SaveToTempDataHandler : ICreateStep2PostHandler 
    { 
        public Task HandleAsync(CreateStep2PostContext context) 
        { 
            context.PageModel.TempData["Observation"] = JsonSerializer.Serialize(context.ObservationVM); 
            return Task.CompletedTask; 
        } 
    } 
}