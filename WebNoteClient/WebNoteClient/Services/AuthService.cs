using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebNoteClient.ApiModels.Auth;
using WebNoteClient.Models.Auth;

namespace WebNoteClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        private readonly string loginRoute = "api/auth/login";
        private readonly string registerRoute = "api/auth/register";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private AuthResult ToSuccessAuthResult(AuthResponse result)
        {
            return new AuthResult
            {
                IsSuccess = true,
                Token = result.token,
                Email = result.username,
                Expires = result.expires,
            };
        }

        public async Task<AuthResult> LoginAsync(LoginViewModel loginModel)
        {
            var model = new LoginRequest { Email = loginModel.Email, Password = loginModel.Password };
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(loginRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return ToSuccessAuthResult(result);
            }
            return new AuthResult { IsSuccess = false };
        }

        public async Task<AuthResult> RegisterAsync(RegisterViewModel registerModel)
        {
            var model = new RegisterRequest { 
                Email = registerModel.Email, 
                Password = registerModel.Password,
                ConfirmPassword = registerModel.ConfirmPassword,
            };
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(registerRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return ToSuccessAuthResult(result);
            }
            return new AuthResult { IsSuccess = false };
        }
    }
}
