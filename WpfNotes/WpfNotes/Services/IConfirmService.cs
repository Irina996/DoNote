using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotes.Services
{
    public interface IConfirmService
    {
        bool ShowConfirmation(string title, string message);
    }
}
