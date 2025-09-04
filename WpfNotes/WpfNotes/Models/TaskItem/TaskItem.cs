using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotes.Models.TaskItem
{
    public class TaskItem : BindableBase
    {
        private int _id;
        private string _content;
        private bool _isCompleted;
        private DateTime _creationDate;
        private DateTime? _notification;
        private TaskCategory _category;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public DateTime CreationDate
        {
            get => _creationDate;
            set => SetProperty(ref _creationDate, value);
        }

        public DateTime? Notification
        {
            get => _notification;
            set => SetProperty(ref _notification, value);
        }

        public TaskCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
    }
}
