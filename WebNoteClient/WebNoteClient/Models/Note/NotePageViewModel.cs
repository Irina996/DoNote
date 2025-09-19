namespace WebNoteClient.Models.Note
{
    public class NotePageViewModel
    {
        public List<NoteCategoryModel> Categories { get; set; }
        public List<NoteModel> Notes { get; set; }

        public int? SelectedCategoryId { get; set; }
    }
}
