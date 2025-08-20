using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebNotes.Entities;

namespace WebNotes.Data {
    public class ApplicationDbContext : IdentityDbContext {

        public DbSet<Note> Notes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
            Database.EnsureCreated();
        }
    }
}
