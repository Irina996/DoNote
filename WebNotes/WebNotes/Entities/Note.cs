using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebNotes.Entities {
    public class Note {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; } 

        public DateTime ChangeDate { get; set; } 

        public bool IsPinned { get; set; } = false;

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
