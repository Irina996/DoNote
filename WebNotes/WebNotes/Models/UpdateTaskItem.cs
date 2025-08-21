using System.ComponentModel.DataAnnotations;

namespace WebNotes.Models {
    public class UpdateTaskItem {

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime? Notification { get; set; } = null;

        [Required]
        public int CategoryId { get; set; }
    }
}
