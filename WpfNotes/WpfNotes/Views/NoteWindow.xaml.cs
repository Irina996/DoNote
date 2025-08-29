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
using System.Windows.Shapes;
using WpfNotes.Models.Note;
using WpfNotes.ViewModels;

namespace WpfNotes.Views
{
    /// <summary>
    /// Логика взаимодействия для NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        private readonly NoteViewModel _viewModel;
        public NoteWindow(Note note, List<Category> categories, bool isNewNote)
        {
            InitializeComponent();
            _viewModel = new NoteViewModel(note, categories, isNewNote);
            DataContext = _viewModel;
            Loaded += NoteWindow_Loaded;
        }

        private void NoteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IChangeWindows vm)
            {
                vm.Change += () =>
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                };
            }
            if (!string.IsNullOrEmpty(NoteTitleTextBox.Text))
            {
                HintNoteTitleTextBlock.Visibility = Visibility.Collapsed;
            }
            if (!string.IsNullOrEmpty(NoteContentTextBox.Text))
            {
                HintNoteContentTextBlock.Visibility = Visibility.Collapsed;
            }
            Console.WriteLine(categoryComboBox.SelectedItem);
        }

        private void NoteTitleBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HintNoteTitleTextBlock.Visibility = Visibility.Collapsed;
        }

        private void NoteTitleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NoteTitleTextBox.Text))
            {
                HintNoteTitleTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void NoteContentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HintNoteContentTextBlock.Visibility = Visibility.Collapsed;
        }

        private void NoteContentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NoteContentTextBox.Text))
            {
                HintNoteContentTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
