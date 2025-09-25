namespace WebNoteClient.Models.TaskItem
{
    public class TaskPageViewModel
    {
        public List<TaskCategoryModel> Categories { get; set; }
        public List<TaskItemModel> TaskItems { get; set; }

        public int? SelectedCategoryId { get; set; }

        public string? SearchQuery { get; set; }

    }
}
