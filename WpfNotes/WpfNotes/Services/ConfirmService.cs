using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfNotes.Services
{
    public class ConfirmService : IConfirmService
    {
        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }
    }
}
