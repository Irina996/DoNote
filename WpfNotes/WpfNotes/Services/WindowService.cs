using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Models.Note;
using WpfNotes.ViewModels;
using WpfNotes.Views;

namespace WpfNotes.Services
{
    public class WindowService : IWindowService
    {
        public void ShowCategoryWindow(CategoryViewModel categoryViewModel)
        {
            CategoryWindow categoryWindow = new CategoryWindow(categoryViewModel);
            categoryWindow.ShowDialog();
        }

        public void ShowNoteWindow(NoteViewModel noteViewModel)
        {
            NoteWindow noteWindow = new NoteWindow(noteViewModel);
            noteWindow.ShowDialog();
        }

        public void ShowTaskWindow(TaskViewModel taskViewModel)
        {
            TaskWindow taskWindow = new TaskWindow(taskViewModel);
            taskWindow.ShowDialog();
        }
    }
}
