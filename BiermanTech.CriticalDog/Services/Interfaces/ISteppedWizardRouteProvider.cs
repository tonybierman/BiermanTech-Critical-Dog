namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface ISteppedWizardRouteProvider
    {
        string GetNextStep(string currentStep);
        string GetPreviousStep(string currentStep);
    }
}