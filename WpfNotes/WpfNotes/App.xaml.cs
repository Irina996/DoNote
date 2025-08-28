using System.Configuration;
using System.Data;
using System.Windows;
using WpfNotes.Services;

namespace WpfNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            if (!string.IsNullOrEmpty(Settings.Default.JwtToken))
            {
                ApiService.GetInstance(Settings.Default.JwtToken);
                new MainWindow().Show();
            }
            else
            {
                new AuthWindow().Show();
            }

            base.OnStartup(e);
        }
    }

}
