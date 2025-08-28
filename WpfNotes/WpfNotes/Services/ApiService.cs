using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.ApiModels.Note;
using WpfNotes.Models.Note;

namespace WpfNotes.Services
{
    public class ApiService
    {
        private static ApiService instance;

        private readonly HttpClient _httpClient;

        private readonly string _token;

        private readonly string notesRoute = "api/notes";
        private readonly string noteCategoriesRoute = "api/categories";
        private readonly string tasksRoute = "api/tasks";
        private readonly string taskCategoriesRoute = "api/taskcategories";

        private ApiService(string token)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration config = builder.Build();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config["ApiSettings:BaseUrl"])
            };
            _token = token;
        }

        public static ApiService GetInstance()
        {
            return instance;
        }

        public static ApiService GetInstance(string token)
        {
            if (instance == null)
                instance = new ApiService(token);
            return instance;
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, notesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Note>>();
            }
            return new List<Note>();
        }

        public async Task<List<Category>> GetNoteCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, noteCategoriesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Category>>();
            }
            return new List<Category>();
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, notesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateNoteRequest model = new CreateNoteRequest
            {
                Title = note.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                CategoryId = note.Category.Id
            };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Note>();
            }
            return null;
        }
    }
}
