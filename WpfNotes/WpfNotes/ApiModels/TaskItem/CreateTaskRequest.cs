namespace WpfNotes.ApiModels.TaskItem
{
    public class CreateTaskRequest
    {
        public string Content { get; set; }

        public DateTime? Notification { get; set; } = null;

        public int CategoryId { get; set; }
    }
}
