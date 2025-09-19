
namespace WebNoteClient.ApiModels.Note
{
    public class CreateNoteRequest
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsPinned { get; set; } = false;

        public int CategoryId { get; set; }
    }
}
