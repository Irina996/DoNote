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
using WpfNotes.ViewModels;

namespace WpfNotes.Views
{
    /// <summary>
    /// Логика взаимодействия для CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        public CategoryWindow(CategoryViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            Loaded += CategoryWindow_Loaded;
        }

        private void CategoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IChangeWindows vm)
            {
                vm.Change += () =>
                {
                    this.Close();
                };
            }
            
            if (!string.IsNullOrEmpty(CategoryTextBox.Text))
            {
                CategoryHintBox.Visibility = Visibility.Collapsed;
            }
        }

        private void CategoryTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CategoryHintBox.Visibility = Visibility.Collapsed;
        }

        private void CategoryTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CategoryTextBox.Text))
            {
                CategoryHintBox.Visibility = Visibility.Visible;
            }
        }
    }
}
