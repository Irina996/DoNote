using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNotes.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; } 
    }

    public class AuthResponse
    {
        public string token { get; set; }
        public string username { get; set; }

        public DateTime expires { get; set; }
    }
}
