using System.ComponentModel.DataAnnotations;

namespace WebNotes.Models {
    public class UpdateNoteModel {

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public bool IsPinned { get; set; } = false;

        [Required]
        public int CategoryId { get; set; }
    }
}
