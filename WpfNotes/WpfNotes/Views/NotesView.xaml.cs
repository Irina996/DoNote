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
using WpfNotes.Models.Note;
using WpfNotes.ViewModels;

namespace WpfNotes.Views
{
    /// <summary>
    /// Логика взаимодействия для NotesView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {

        public NotesView()
        {
            InitializeComponent();
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
