using System.Threading.Tasks;

namespace BiermanTech.CriticalDog.Pages.Dogs.Observations.Step2GetPipeline
{
    public class PopulateViewModelStage : ICreateStep2GetStage
    {
        private readonly CreateStep2Model _pageModel;

        public PopulateViewModelStage(CreateStep2Model pageModel)
        {
            _pageModel = pageModel;
        }

        public async Task HandleAsync(Step2GetContext context)
        {
            _pageModel.PopulateSelectListItems(context.ObservationDefinition);
            context.PageModel = _pageModel; // Update context with populated PageModel
        }
    }
}