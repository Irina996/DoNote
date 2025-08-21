using System.ComponentModel.DataAnnotations;
using WebNotes.Entities;

namespace WebNotes.Models {
    public class TaskModel {
        public int Id { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreationDate { get; set; }

        public DateTime? Notification { get; set; } = null;

        public CategoryModel Category { get; set; }
    }
}
