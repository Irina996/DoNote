using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfNotes.Commands;
using WpfNotes.Models.TaskItem;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class TasksListViewModel : ViewModelBase
    {
        private readonly ReminderService _reminderService;
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
                _reminderService.Stop();
                _reminderService.SetTasks(Tasks);
                _reminderService.Start();
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
            _reminderService = new ReminderService();
            _windowService = new WindowService();
            _tasksModel = new TasksListModel();
            _tasksModel.PropertyChanged += OnTasksModelPropertyChanged;

            LoadData();
            LoadCommand = new AsyncRelayCommand(LoadData);
            CreateTaskCommand = new RelayCommand(CreateTask);
            EditTaskCommand = new RelayCommand(EditTask);
            CreateCategoryCommand = new RelayCommand(CreateCategory);
            EditCategoryCommand = new RelayCommand(EditCategory);
            SearchTaskCommand = new RelayCommand(FilterTasks);
            ChangeSelectedCategoryCommand = new RelayCommand(FilterTasks);

            _allTasks = new ObservableCollection<TaskItem>();
            _tasks = new ObservableCollection<TaskItem>();
            _categories = new ObservableCollection<TaskCategory>();
            _searchText = "";

            _reminderService.SetTasks(Tasks);
            _reminderService.Start();
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

        private async Task LoadData()
        {
            IsLoading = true;
            await _tasksModel.LoadAsync();
            IsLoading = false;
        }

        private async Task OpenTaskWindow(TaskItem task, bool isNewTask)
        {
            List<TaskCategory> categories = new List<TaskCategory>(_categories);
            categories.RemoveAt(0); // remove "All" category
            _windowService.ShowTaskWindow(new TaskViewModel(task, categories, isNewTask));
            if (isNewTask && !string.IsNullOrEmpty(task.Content))
            {
                _tasksModel.AddTask(task);
            }
            if (!isNewTask && string.IsNullOrEmpty(task.Content))
            {
                _tasksModel.RemoveTask(task);
            }
            FilterTasks();
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

        private void CreateTask()
        {
            var task = new TaskItem();
            if (SelectedCategory.Id == -1)
            {
                task.Category = Categories[1];
            }
            else
            {
                task.Category = SelectedCategory;
            }
            OpenTaskWindow(task, true);
        }

        private void EditTask()
        {
            OpenTaskWindow(SelectedTask, false);
        }

        private void CreateCategory()
        {
            OpenCategoryWindowAsync(new TaskCategory(), true);
        }

        private void EditCategory()
        {
            OpenCategoryWindowAsync(SelectedCategory, false);
        }

        private void FilterTasks()
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
