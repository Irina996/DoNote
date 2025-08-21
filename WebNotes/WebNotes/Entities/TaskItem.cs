using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebNotes.Entities {
    public class TaskItem {
        public int Id { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public DateTime? Notification { get; set; } = null;

        public int CategoryId { get; set; }

        public virtual TaskCategory Category { get; set; }
        
    }
}
