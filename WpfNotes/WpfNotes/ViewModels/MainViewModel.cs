using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfNotes.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public NotesListViewModel NotesViewModel { get; set; }

        public TasksListViewModel TasksViewModel { get; set; }

        public MainViewModel() 
        {
            NotesViewModel = new NotesListViewModel();
            TasksViewModel = new TasksListViewModel();
        }
    }
}