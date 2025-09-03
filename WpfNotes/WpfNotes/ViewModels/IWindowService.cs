using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Models.Note;

namespace WpfNotes.ViewModels
{
    interface IWindowService<T, K>
    {
        public Func<T, List<K>, bool, Task> OpenItemWindowAsyncFunc { get; set; }
        public Func<K, bool, Task> OpenCategoryWindowAsyncFunc { get; set; }
    }
}
