using System.ComponentModel.DataAnnotations;

namespace WebNotes.Models {
    public class CreateCategoryModel {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
