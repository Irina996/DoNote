using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNotes.Models.TaskItem;
using WpfNotes.Services;

namespace WpfNotes.ViewModels
{
    public class TaskViewModel : ViewModelBase, IChangeWindows
    {
        private readonly TaskModel _taskModel;
        private readonly IConfirmService _confirmService;

        private TaskItem _task;
        private ObservableCollection<TaskCategory> _categories;
        private bool isNewTask;

        public TaskItem TaskItem
        {
            get => _task;
            set
            {
                _task = value;
                OnPropertyChanged(nameof(TaskItem));
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

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand NotificationCommand { get; }

        public Action Change { get; set; }

        public TaskViewModel(TaskItem task, List<TaskCategory> categories, bool isNew)
        {
            _taskModel = new TaskModel(task, categories);
            _taskModel.PropertyChanged += OnModelPropertyChanged;
            _categories = new ObservableCollection<TaskCategory>(_taskModel.Categories);
            _task = _taskModel.TaskItem;

            isNewTask = isNew;

            _confirmService = new ConfirmService();

            SaveCommand = new ViewModelCommand(SaveTask);
            DeleteCommand = new ViewModelCommand(DeleteTask);
            CancelCommand = new ViewModelCommand(Cancel);
            BackCommand = new ViewModelCommand(Back);
            NotificationCommand = new ViewModelCommand(SetNotification);
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_taskModel.TaskItem))
            {
                TaskItem = _taskModel.TaskItem; 
            }
        }

        private async void SaveTask(object obj)
        {
            if (_confirmService.ShowConfirmation("Confirm", "Save the task?"))
            {
                if (isNewTask)
                {
                    await _taskModel.CreateTask();
                    isNewTask = false;
                }
                else
                {
                    await _taskModel.UpdateTask();
                }
            }
        }

        private async void DeleteTask(object obj)
        {
            if (_confirmService.ShowConfirmation("Confirm", "Delete the task?"))
            {
                if (!isNewTask)
                {
                    await _taskModel.DeleteTask();
                }
                CloseWindow();
            }
        }

        private void Cancel(object obj)
        {
            if (_confirmService.ShowConfirmation("Confirm", "Cancel?"))
            {
                _taskModel.CancelChanges();
            }
        }

        private void SetNotification(object obj)
        {
            throw new NotImplementedException();
        }

        private void Back(object obj)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Change?.Invoke();
        }
    }
}
