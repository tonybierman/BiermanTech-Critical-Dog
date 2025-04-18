using System.Text.Json;
using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline
{ 
    public class SaveToTempDataStage : ICreateStep2PostStage 
    { 
        public Task HandleAsync(CreateStep2PostContext context) 
        { 
            context.PageModel.TempData["Observation"] = JsonSerializer.Serialize(context.ObservationVM); 
            return Task.CompletedTask; 
        } 
    } 
}