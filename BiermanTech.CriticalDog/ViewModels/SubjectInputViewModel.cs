using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class SubjectInputViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public sbyte Sex { get; set; }

        public DateOnly? ArrivalDate { get; set; }

        public int? SubjectTypeId { get; set; }

        public virtual SubjectType? SubjectType { get; set; }
    }
}
