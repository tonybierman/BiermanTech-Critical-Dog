using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.Services.SteppedWizardProviders
{
    public class DefaultSteppedWizardRouteProvider : ISteppedWizardRouteProvider
    {
        private readonly string[] _steps = { "CreateStep1", "CreateStep2", "CreateStep3" };

        public string GetNextStep(string currentStep)
        {
            int currentIndex = Array.IndexOf(_steps, currentStep);
            if (currentIndex < 0 || currentIndex >= _steps.Length - 1)
            {
                return null; // No next step if current step is invalid or last
            }
            return _steps[currentIndex + 1];
        }

        public string GetPreviousStep(string currentStep)
        {
            int currentIndex = Array.IndexOf(_steps, currentStep);
            if (currentIndex <= 0) // No previous step if current step is invalid or first
            {
                return null;
            }
            return _steps[currentIndex - 1];
        }
    }
}