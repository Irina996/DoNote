using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.TaskItem
{
    public class TaskCategoryModel : BindableBase
    {
        private readonly ApiService _apiService;

        private TaskCategory _prevCategory;
        private TaskCategory _category;

        public TaskCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private TaskCategoryModel()
        {
            _apiService = ApiService.GetInstance();
        }

        public TaskCategoryModel(TaskCategory category) : this()
        {
            _prevCategory = category;
            CopyVersion();
        }

        private void CopyVersion()
        {
            Category = new TaskCategory
            {
                Id = _prevCategory.Id,
                Name = _prevCategory.Name
            };
        }

        public async Task CreateCategory()
        {
            Category = await _apiService.CreateTaskCategoryAsync(Category);
            _prevCategory.Id = Category.Id;
            _prevCategory.Name = Category.Name;
        }

        public async Task UpdateCategory()
        {
            await _apiService.UpdateTaskCategoryAsync(Category);
            _prevCategory.Name = Category.Name;
        }

        public async Task DeleteCategory()
        {
            await _apiService.DeleteTaskCategoryAsync(Category);
            _prevCategory.Name = null;
        }

        public void CancelChanges()
        {
            CopyVersion();
        }
    }
}
