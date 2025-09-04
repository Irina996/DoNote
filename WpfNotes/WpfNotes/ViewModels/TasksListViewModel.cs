using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfNotes.Models.TaskItem;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class TasksListViewModel : ViewModelBase
    {
        private readonly IWindowService _windowService;
        private readonly TasksListModel _tasksModel;

        private ObservableCollection<TaskItem> _allTasks;
        private ObservableCollection<TaskItem> _tasks;
        private ObservableCollection<TaskCategory> _categories;
        private TaskItem _selectedTask;
        private TaskCategory _selectedCategory;
        private string _searchText;
        private bool _isLoading;

        public ObservableCollection<TaskItem> AllTasks 
        { 
            get => _allTasks;
            set 
            { 
                _allTasks = value; 
                OnPropertyChanged(nameof(AllTasks)); 
            }
        }
        public ObservableCollection<TaskItem> Tasks
        { 
            get => _tasks; 
            set 
            { 
                _tasks = value; 
                OnPropertyChanged(nameof(Tasks)); 
            } 
        }
        public ObservableCollection<TaskCategory> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }
        public TaskCategory SelectedCategory 
        { 
            get => _selectedCategory; 
            set
            { 
                _selectedCategory = value; 
                OnPropertyChanged(nameof(SelectedCategory));
            } 
        }
        public string SearchText 
        { 
            get => _searchText; 
            set 
            { 
                _searchText = value; 
                OnPropertyChanged(nameof(SearchText));
            } 
        }
        public bool IsLoading 
        { 
            get => _isLoading; 
            set 
            {
                _isLoading = value; 
                OnPropertyChanged(nameof(IsLoading));
            } 
        }

        public ICommand LoadCommand { get; }
        public ICommand CreateTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand CreateCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand SearchTaskCommand { get; }
        public ICommand ChangeSelectedCategoryCommand { get; }

        public TasksListViewModel()
        {
            _windowService = new WindowService();
            _tasksModel = new TasksListModel();
            _tasksModel.PropertyChanged += OnTasksModelPropertyChanged;

            LoadData(null);
            LoadCommand = new ViewModelCommand(LoadData);
            CreateTaskCommand = new ViewModelCommand(CreateTask);
            EditTaskCommand = new ViewModelCommand(EditTask);
            CreateCategoryCommand = new ViewModelCommand(CreateCategory);
            EditCategoryCommand = new ViewModelCommand(EditCategory);
            SearchTaskCommand = new ViewModelCommand(FilterTasks);
            ChangeSelectedCategoryCommand = new ViewModelCommand(FilterTasks);

            _allTasks = new ObservableCollection<TaskItem>();
            _tasks = new ObservableCollection<TaskItem>();
            _categories = new ObservableCollection<TaskCategory>();
            _searchText = "";
        }

        private void OnTasksModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TasksListModel.AllTasks))
            {
                AllTasks.Clear();
                foreach(var task in _tasksModel.AllTasks)
                {
                    AllTasks.Add(task);
                }
            }
            else if (e.PropertyName == nameof(TasksListModel.Tasks))
            {
                Tasks.Clear();
                foreach (var task in _tasksModel.Tasks)
                {
                    Tasks.Add(task);
                }
                SelectedTask = Tasks.FirstOrDefault();
            }
            else if (e.PropertyName == nameof(_tasksModel.Categories))
            {
                _categories.Clear();
                // add "All" category
                _categories.Add(new TaskCategory { Id = -1, Name = "All" });
                _selectedCategory = Categories[0];
                foreach (var category in _tasksModel.Categories)
                {
                    _categories.Add(category);
                }
                OnPropertyChanged(nameof(Categories));
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private async void LoadData(object obj)
        {
            IsLoading = true;
            await _tasksModel.LoadAsync();
            IsLoading = false;
        }

        private async Task OpenTaskWindow(TaskItem task, bool isNewTask)
        {
            List<TaskCategory> categories = new List<TaskCategory>(_categories);
            categories.RemoveAt(0); // remove "All" category
            // TODO: open task window
            //_windowService.ShowTaskWindow(new TaskViewMode(task, categories, isNewTask));
            if (isNewTask && !string.IsNullOrEmpty(task.Content))
            {
                _tasksModel.AddTask(task);
            }
            if (!isNewTask && string.IsNullOrEmpty(task.Content))
            {
                _tasksModel.RemoveTask(task);
            }
            FilterTasks(null);
        }

        private async Task OpenCategoryWindowAsync(TaskCategory category, bool isNewCategory)
        {
            _windowService.ShowCategoryWindow(new TaskCategoryViewModel(category, isNewCategory));
            if (isNewCategory && !string.IsNullOrEmpty(category.Name))
            {
                Categories.Add(category);
            }
            if (!isNewCategory && string.IsNullOrEmpty(category.Name))
            {
                Categories.Remove(category);
            }
        }

        private void CreateTask(object obj)
        {
            OpenTaskWindow(new TaskItem(), true);
        }

        private void EditTask(object obj)
        {
            OpenTaskWindow(SelectedTask, false);
        }

        private void CreateCategory(object obj)
        {
            OpenCategoryWindowAsync(new TaskCategory(), true);
        }

        private void EditCategory(object obj)
        {
            OpenCategoryWindowAsync(SelectedCategory, false);
        }

        private void FilterTasks(object obj)
        {
            SelectedCategory ??= Categories[0];
            if (SelectedCategory.Id == -1)
            {
                // All category
                _tasksModel.FilterTasks(SearchText);
            }
            else
            {
                _tasksModel.FilterTasks(SearchText, SelectedCategory);
            }
        }
    }
}
