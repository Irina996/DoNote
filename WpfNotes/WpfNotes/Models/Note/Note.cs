using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotes.Models.Note
{
    public class Note : BindableBase
    {
        private int _id;
        private string _title;
        private string _content;
        private DateTime _creationDate;
        private DateTime _changeDate;
        private bool _isPinned;
        private Category _category;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public DateTime CreationDate
        {
            get => _creationDate;
            set => SetProperty(ref _creationDate, value);
        }

        public DateTime ChangeDate
        {
            get => _changeDate;
            set => SetProperty(ref _changeDate, value);
        }

        public bool IsPinned
        {
            get => _isPinned;
            set => SetProperty(ref _isPinned, value);
        }

        public Category Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
    }
}
