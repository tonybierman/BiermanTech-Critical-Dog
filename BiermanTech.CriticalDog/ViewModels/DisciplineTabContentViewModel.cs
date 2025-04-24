using BiermanTech.CriticalDog.Data;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class DisciplineTabContentViewModel
    {
        public string DisciplineKey { get; set; }
        public List<SubjectRecordViewModel> Records { get; set; }
    }
}
