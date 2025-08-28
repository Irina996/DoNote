using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Services;

namespace WpfNotes.Models.Auth
{
    public class AuthModel
    {
        private readonly AuthService _authService;

        public AuthModel()
        {
            _authService = new AuthService();
        }

        // TODO: rewrite to notify about result
        public async Task<bool> LoginAsync(string email, string password, bool isRememberMe)
        {
            var token = await _authService.LoginAsync(new LoginModel { Email = email, Password = password });
            if (token != null)
            {
                if (isRememberMe)
                {
                    Settings.Default.JwtToken = token;
                    Settings.Default.Save();
                }
                ApiService.GetInstance(token);
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password, string confirmPassword, bool isRememberMe)
        {
            var token = await _authService.RegisterAsync(
                new RegisterModel
                {
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword
                });
            if (token != null)
            {
                if (isRememberMe)
                {
                    Settings.Default.JwtToken = token;
                    Settings.Default.Save();
                }
                ApiService.GetInstance(token);
                return true;
            }
            return false;
        }
    }
}
