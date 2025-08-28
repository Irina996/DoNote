using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfNotes.ViewModels;
using WpfNotes.Views;

namespace WpfNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.NotesViewModel.OpenNoteWindowAction += (note, categories) =>
            {
                throw new NotImplementedException();
                /*
                NoteWindow noteWindow = new NoteWindow(note, categories);
                noteWindow.Show();
                */
            };
            _viewModel.NotesViewModel.OpenCategoryWindowAction += (category) =>
            {
                throw new NotImplementedException();
                /*
                CategoryWindow categoryWindow = new CategoryWindow(category);
                categoryWindow.Show();
                */
            };
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