using System.ComponentModel.DataAnnotations;

namespace WebNotes.Models {
    public class CreateTaskModel {
        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Content { get; set; }

        public DateTime? Notification { get; set; } = null;

        [Required]
        public int CategoryId { get; set; }
    }
}
