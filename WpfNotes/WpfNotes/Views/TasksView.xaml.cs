using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfNotes.Models.TaskItem;
using WpfNotes.ViewModels;

namespace WpfNotes.Views
{
    /// <summary>
    /// Логика взаимодействия для TasksView.xaml
    /// </summary>
    public partial class TasksView : UserControl
    {
        public TasksView()
        {
            InitializeComponent();

            Loaded += TasksView_Loaded;
        }

        private void TasksView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IWindowService<TaskItem, TaskCategory> windowService)
            {
                windowService.OpenItemWindowAsyncFunc += OpenTaskWindowAsync;
                windowService.OpenCategoryWindowAsyncFunc += OpenCategoryWindowAsync;
            }
        }

        private Task OpenTaskWindowAsync(TaskItem task, List<TaskCategory> categories, bool isNewTask)
        {
            throw new NotImplementedException();
        }

        private Task OpenCategoryWindowAsync(TaskCategory category, bool isNewCategory)
        {
            throw new NotImplementedException();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                HintSearchTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                HintSearchTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
