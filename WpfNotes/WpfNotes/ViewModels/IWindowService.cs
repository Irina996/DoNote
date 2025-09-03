using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Models.Note;

namespace WpfNotes.ViewModels
{
    interface IWindowService
    {
        public Func<Note, List<Category>, bool, Task> OpenItemWindowAsyncFunc { get; set; }
        public Func<Category, bool, Task> OpenCategoryWindowAsyncFunc { get; set; }
    }
}
