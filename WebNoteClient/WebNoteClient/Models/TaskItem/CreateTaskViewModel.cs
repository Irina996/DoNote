namespace WebNoteClient.Models.TaskItem
{
    public class CreateTaskViewModel
    {
        public TaskItemModel TaskItem { get; set; }

        public List<TaskCategoryModel> Categories { get; set; }
    }
}
