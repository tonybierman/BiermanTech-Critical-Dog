namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface ISteppedWizardRouteProvider
    {
        string GetFirstStep();
        string GetNextStep(string currentStep);
        string GetPreviousStep(string currentStep);
        string GetOutStep();
    }
}