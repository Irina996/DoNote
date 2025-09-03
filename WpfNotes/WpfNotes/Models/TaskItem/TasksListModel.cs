using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.TaskItem
{
    public class TasksListModel : BindableBase
    {
        private readonly ApiService _apiService;

        private List<TaskItem> _allTasks = new List<TaskItem>();
        private List<TaskItem> _tasks = new List<TaskItem>();
        private List<TaskCategory> _categories = new List<TaskCategory>();

        public List<TaskItem> AllTasks { get => _allTasks; set => SetProperty(ref _allTasks, value); }
        public List<TaskItem> Tasks { get => _tasks; set => SetProperty(ref _tasks, value); }
        public List<TaskCategory> Categories { get => _categories; set => SetProperty(ref _categories, value); }

        public TasksListModel()
        {
            _apiService = ApiService.GetInstance();
        }

        public async Task LoadAsync()
        {
            try
            {
                _allTasks = (await _apiService.GetTaskItemsAsync())
                    .OrderByDescending(t => t.CreationDate).ToList();
                Tasks = _allTasks;

                var categories = await _apiService.GetTaskCategoriesAsync();
                Categories = categories;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public void AddTask(TaskItem task)
        {
            AllTasks.Insert(0, task);
        }

        public void RemoveTask(TaskItem task) 
        { 
            AllTasks.Remove(task); 
        }

        public async Task FilterTasks(string searchText = null, TaskCategory category = null)
        {
            IEnumerable<TaskItem> tasksFilteredByText = AllTasks;
            if (searchText != null)
            {
                tasksFilteredByText = AllTasks
                    .Where(t => t.Content.Contains(searchText));
            }
            IEnumerable<TaskItem> tasksFilteredByCategory = AllTasks;
            if (category != null)
            {
                tasksFilteredByCategory = AllTasks
                    .Where(t => t.Category == category);
            }
            Tasks = tasksFilteredByText
                .Intersect(tasksFilteredByCategory)
                .OrderByDescending(t => t.CreationDate)
                .ToList();
        }
    }
}
