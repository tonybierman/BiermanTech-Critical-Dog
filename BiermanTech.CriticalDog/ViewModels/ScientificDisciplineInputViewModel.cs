namespace BiermanTech.CriticalDog.ViewModels
{
    public class ScientificDisciplineInputViewModel
    {
        public int Id { get; set; }

        public string DisciplineName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}