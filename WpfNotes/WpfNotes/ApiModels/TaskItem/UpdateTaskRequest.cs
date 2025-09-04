namespace WpfNotes.ApiModels.TaskItem
{
    public class UpdateTaskRequest
    {
        public string Content { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Notification { get; set; } = null;
        public int CategoryId { get; set; }
    }
}
