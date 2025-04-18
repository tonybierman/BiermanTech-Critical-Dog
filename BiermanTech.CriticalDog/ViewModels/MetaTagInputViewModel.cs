﻿using System.ComponentModel.DataAnnotations;

namespace BiermanTech.CriticalDog.ViewModels
{
    public class MetaTagInputViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TagName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}