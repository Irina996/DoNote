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

namespace WpfNotes {
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window {
        public AuthWindow() {
            InitializeComponent();
            Loaded += AuthWindow_Loaded;
        }

        private void AuthWindow_Loaded(object sender, RoutedEventArgs e)
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
        }

        private void OpenMainWindow()
        {

        }

        private void NavigateToRegister_Click(object sender, RoutedEventArgs e) {
            registerTab.IsSelected = true;
        }

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e) {
            loginTab.IsSelected = true;
        }

        private async void Login_Click(object sender, RoutedEventArgs e) {
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }
    }
}
