namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IObservationWizardRouteFactory
    {
        ISteppedWizardRouteProvider GetStep2Route(string? slug);
    }
}