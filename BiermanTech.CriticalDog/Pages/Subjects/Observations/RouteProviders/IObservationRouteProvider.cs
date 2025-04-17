using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.RouteProviders
{
    public interface IObservationRouteProvider
    {
        bool CanHandle(Subject dog, string typeName);
        string GetRoute();
    }
}
