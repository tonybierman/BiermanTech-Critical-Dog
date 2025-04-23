namespace BiermanTech.CriticalDog.ViewModels
{
    public interface ICardViewModel
    {
        string Title { get; }
        Task Init();
        bool CanHandle();
    }
}
