using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.ApiModels.Auth;

namespace WpfNotes.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        private string _token;

        private readonly string loginRoute = "api/auth/login";
        private readonly string registerRoute = "api/auth/register";

        public AuthService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration config = builder.Build();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(config["ApiSettings:BaseUrl"]);
        }

        public async Task<string> LoginAsync(LoginRequest model)
        {
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(loginRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _token = result?.token;
                return _token;
            }
            return null;
        }

        public async Task<string> RegisterAsync(RegisterRequest model)
        {
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(registerRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _token = result?.token;
                return _token;
            }
            return null;
        }
    }
}
