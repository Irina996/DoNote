namespace WebNoteClient.Models.Note
{
    public class NoteModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public bool IsPinned { get; set; } = false;

        public NoteCategoryModel Category { get; set; }
    }
}
