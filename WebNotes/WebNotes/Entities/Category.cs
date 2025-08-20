using Microsoft.AspNetCore.Identity;

namespace WebNotes.Entities {
    public class Category {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }

        public virtual IdentityUser User { get; set; }
        public virtual ICollection<Note> Notes { get; set; } 
    }
}
