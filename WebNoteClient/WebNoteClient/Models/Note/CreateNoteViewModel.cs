namespace WebNoteClient.Models.Note
{
    public class CreateNoteViewModel
    {
        public NoteModel Note { get; set; }

        public List<NoteCategoryModel> Categories { get; set; }
    }
}
