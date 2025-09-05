using WpfNotes.ApiModels.Category;

namespace WpfNotes.ApiModels.TaskItem
{
    public class GetTaskResponse
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreationDate { get; set; }

        public DateTime? Notification { get; set; } = null; // in UTC

        public GetCategoryResponse Category { get; set; }
    }
}
