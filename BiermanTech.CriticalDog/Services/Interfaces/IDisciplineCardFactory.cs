namespace BiermanTech.CriticalDog.Services.Interfaces
{
    public interface IDisciplineCardFactory
    {
        IDisciplineCardProvider? CreateProvider(int subjectId, string slug);
    }
}
