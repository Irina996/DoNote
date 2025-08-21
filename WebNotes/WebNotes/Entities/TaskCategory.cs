using Microsoft.AspNetCore.Identity;

namespace WebNotes.Entities {
    public class TaskCategory {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }

        public virtual IdentityUser User { get; set; }
        public virtual ICollection<TaskItem> Tasks { get; set; }
    }
}
