using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WpfNotes.ApiModels.Category;
using WpfNotes.ApiModels.Note;
using WpfNotes.ApiModels.TaskItem;
using WpfNotes.Models.Note;
using WpfNotes.Models.TaskItem;

namespace WpfNotes.Services
{
    public class ApiService
    {
        private static ApiService instance;

        private readonly HttpClient _httpClient;

        private readonly string _token;

        private readonly string notesRoute = "api/notes/";
        private readonly string noteCategoriesRoute = "api/categories/";
        private readonly string tasksRoute = "api/tasks/";
        private readonly string taskCategoriesRoute = "api/taskcategories/";

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

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, notesRoute + note.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            UpdateNoteRequest model = new UpdateNoteRequest
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
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteNoteAsync(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, notesRoute + note.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<Category> CreateNoteCategoryAsync(Category category)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, noteCategoriesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateCategoryRequest model = new CreateCategoryRequest { Name = category.Name };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Category>();
            }
            return null;
        }

        public async Task<bool> UpdateNoteCategoryAsync(Category category)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateCategoryRequest model = new CreateCategoryRequest { Name = category.Name };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteNoteCategoryAsync(Category category)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<TaskItem>> GetTaskItemsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, tasksRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            }
            return new List<TaskItem>();
        }

        public async Task<List<TaskCategory>> GetTaskCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, taskCategoriesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TaskCategory>>();
            }
            return new List<TaskCategory>();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, tasksRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateTaskRequest model = new CreateTaskRequest
            {
                Content = task.Content,
                Notification = task.Notification?.ToUniversalTime(),
                CategoryId = task.Category.Id,
            };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskItem>();
            }
            return null;
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, tasksRoute + task.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            UpdateTaskRequest model = new UpdateTaskRequest
            {
                Content = task.Content,
                IsCompleted = task.IsCompleted,
                Notification = task.Notification,
                CategoryId = task.Category.Id,
            };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteTaskAsync(TaskItem task)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, tasksRoute + task.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<TaskCategory> CreateTaskCategoryAsync(TaskCategory category)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, taskCategoriesRoute);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateCategoryRequest model = new CreateCategoryRequest { Name = category.Name };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskCategory>();
            }
            return null;
        }

        public async Task<bool> UpdateTaskCategoryAsync(TaskCategory category)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            CreateCategoryRequest model = new CreateCategoryRequest { Name = category.Name };
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteTaskCategoryAsync(TaskCategory category)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
