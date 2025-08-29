using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotes.ViewModels
{
    interface IConfirm
    {
        Predicate<string> Confirm { get; set; }
    }
}
