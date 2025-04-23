using BiermanTech.CriticalDog.Data;
using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public sbyte Sex { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public int? SubjectTypeId { get; set; }
        public virtual SubjectType? SubjectType { get; set; }
        public string UserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int> SelectedMetaTagIds { get; set; } = new List<int>();
        public List<string> SelectedMetaTagNames { get; set; } = new List<string>();
        public int Permissions { get; set; }
    }
}