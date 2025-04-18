using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.Step2
{
    public interface ICreateStep2PostHandler
    {
        Task HandleAsync(CreateStep2PostContext context);
    }
}