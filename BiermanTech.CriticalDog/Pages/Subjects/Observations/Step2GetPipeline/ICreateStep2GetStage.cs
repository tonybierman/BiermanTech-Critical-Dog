using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public interface ICreateStep2GetStage
    {
        Task HandleAsync(Step2GetContext context);
    }
}