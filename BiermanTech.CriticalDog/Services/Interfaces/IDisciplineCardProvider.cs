using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IDisciplineCardProvider
    {
        string PartialName { get; }
        IDisciplineCardViewModel ViewModel { get; set; }
        Task Init();
        bool CanHandle();
    }
}
