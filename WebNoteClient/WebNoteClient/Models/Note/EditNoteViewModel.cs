namespace WebNoteClient.Models.Note
{
    public class EditNoteViewModel
    {
        public NoteModel Note { get; set; }

        public List<NoteCategoryModel> Categories { get; set; }
    }
}
