using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.TaskItem
{
    public class TaskModel : BindableBase
    {
        private readonly ApiService _apiService;

        private TaskItem _prevTask;
        private TaskItem _task;
        private List<TaskCategory> _categories;

        public TaskItem TaskItem
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }
        public List<TaskCategory> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public TaskModel(TaskItem task, List<TaskCategory> categories)
        {
            _apiService = ApiService.GetInstance();

            _prevTask = task;
            _categories = categories;
            if (_prevTask.Category != null)
            {
                _prevTask.Category = Categories.FirstOrDefault(c => c.Id == _prevTask.Category.Id);
            }
            else
            {
                _prevTask.Category = Categories[0];
            }
            CopyVersion();
        }

        private void CopyVersion()
        {
            TaskItem = new TaskItem
            {
                Id = _prevTask.Id,
                Content = _prevTask.Content,
                CreationDate = _prevTask.CreationDate,
                Category = _prevTask.Category,
                IsCompleted = _prevTask.IsCompleted,
                Notification = _prevTask.Notification,
            };
        }

        private void UpdatePrevVersion(TaskItem task)
        {
            _prevTask.Id = task.Id;
            _prevTask.Content = task.Content;
            _prevTask.CreationDate = task.CreationDate;
            _prevTask.Category = task.Category;
            _prevTask.IsCompleted = task.IsCompleted;
            _prevTask.Notification = task.Notification;
        }

        public async Task CreateTask()
        {
            var task = await _apiService.CreateTaskAsync(TaskItem);
            UpdatePrevVersion(task);
            CopyVersion();
        }

        public async Task UpdateTask()
        {
            await _apiService.UpdateTaskAsync(TaskItem);
            UpdatePrevVersion(TaskItem);
        }

        public async Task DeleteTask()
        {
            await _apiService.DeleteTaskAsync(TaskItem);
            _prevTask.Content = null;
        }

        public async Task CancelChanges()
        {
            CopyVersion();
        }
    }
}
