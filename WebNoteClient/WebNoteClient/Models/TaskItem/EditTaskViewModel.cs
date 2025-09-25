namespace WebNoteClient.Models.TaskItem
{
    public class EditTaskViewModel
    {
        public TaskItemModel TaskItem { get; set; }

        public List<TaskCategoryModel> Categories { get; set; }
    }
}
