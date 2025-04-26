using BiermanTech.CriticalDog.Services.DisciplineCardProviders;
using BiermanTech.CriticalDog.Services.Interfaces;

namespace BiermanTech.CriticalDog.Services.Factories
{
    public class DisciplineTabFormatFactory : IDisciplineTabFormatFactory
    {
        public DisciplineTabFormatFactory()
        {
        }

        public string GetPartialName(string slug)
        {
            if (slug == null)
                return null;

            return slug switch
            {
                "VeterinaryMedicine" or "Biology" => "_DisciplineTabFormatTablePartial",
                _ => "_DisciplineTabFormatListPartial"
            };
        }
    }
}
