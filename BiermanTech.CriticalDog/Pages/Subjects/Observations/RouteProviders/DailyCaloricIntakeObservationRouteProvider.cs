using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Pages.Subjects.Observations.RouteProviders
{
    public class DailyCaloricIntakeObservationRouteProvider : IObservationRouteProvider
    {
        public bool CanHandle(Subject dog, string typeName) => typeName == "DailyCaloricIntake";
        public string GetRoute() => "CreateBehaviorObservation";
    }
}
