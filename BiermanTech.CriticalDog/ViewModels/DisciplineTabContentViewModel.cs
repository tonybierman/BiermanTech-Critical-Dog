using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class DisciplineTabContentViewModel
    {
        public int? SubjectId { get; set; }
        public string? DisciplineKey { get; set; }
        public string? TabFormatPartialName { get; set; }
        public List<SubjectRecordViewModel>? Records { get; set; }
    }
}
