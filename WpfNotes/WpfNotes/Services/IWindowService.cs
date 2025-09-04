using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.ViewModels;

namespace WpfNotes.Services
{
    public interface IWindowService
    {
        void ShowCategoryWindow(CategoryViewModel categoryViewModel);

        void ShowNoteWindow(NoteViewModel noteViewModel);

        // void ShowTaskWindow(TaskViewModel taskViewModel);
    }
}
