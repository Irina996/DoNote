namespace WebNoteClient.Models.TaskItem
{
    public class TaskItemModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? Notification {  get; set; }

        public TaskCategoryModel Category { get; set; }
    }
}
