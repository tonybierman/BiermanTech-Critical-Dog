namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IObservationWizardRouteFactory
    {
        ISteppedWizardRouteProvider GetRouter(string? slug);
    }
}