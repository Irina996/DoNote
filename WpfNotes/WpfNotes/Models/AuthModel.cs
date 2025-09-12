using WpfNotes.ApiModels.Auth;
using WpfNotes.Services;

namespace WpfNotes.Models
{
    public class AuthModel
    {
        private readonly AuthService _authService;

        public AuthModel()
        {
            _authService = new AuthService();
        }

        public async Task<bool> LoginAsync(string email, string password, bool isRememberMe)
        {
            var token = await _authService.LoginAsync(new LoginRequest { Email = email, Password = password });
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
                new RegisterRequest
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
