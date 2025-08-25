using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.Models;

namespace WpfNotes.Services
{
    class AuthService
    {
        private readonly HttpClient _httpClient;

        private readonly string apiUrl;

        private readonly string loginRoute = "/auth/login";
        private readonly string registerRoute = "auth/register";

        public AuthService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            apiUrl = config["ApiSettings:BaseUrl"];

            _httpClient = new HttpClient();
        }

        public async Task<string> LoginAsync(LoginModel model)
        {
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(apiUrl + loginRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return result?.token;
            }
            return null;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            JsonContent content = JsonContent.Create(model);
            using var response = await _httpClient.PostAsync(apiUrl + registerRoute, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return result?.token;
            }
            return null;
        }
    }
}
