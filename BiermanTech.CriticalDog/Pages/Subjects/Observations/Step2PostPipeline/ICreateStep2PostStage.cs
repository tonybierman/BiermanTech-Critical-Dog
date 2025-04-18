using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2PostPipeline
{
    public interface ICreateStep2PostStage
    {
        Task HandleAsync(CreateStep2PostContext context);
    }
}