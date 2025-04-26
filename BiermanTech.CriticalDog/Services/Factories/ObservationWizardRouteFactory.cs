using BiermanTech.CriticalDog.Services.Interfaces;
using BiermanTech.CriticalDog.Services.SteppedWizardProviders;

namespace BiermanTech.CriticalDog.Services.Factories
{
    public class ObservationWizardRouteFactory : IObservationWizardRouteFactory
    {
        public ObservationWizardRouteFactory()
        {
        }

        public ISteppedWizardRouteProvider GetStep2Route(string? slug)
        {
            if (slug == null)
                return null;

            return slug switch
            {
                _ => new DefaultSteppedWizardRouteProvider()
            };
        }
    }
}
