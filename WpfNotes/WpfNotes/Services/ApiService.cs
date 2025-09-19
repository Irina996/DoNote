using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

            var baseUrl = config["ApiSettings:BaseUrl"];
            if (baseUrl == null) { throw new InvalidOperationException("Base address not set."); }
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            if (token == null) { throw new InvalidOperationException("Token not set"); }
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

        private Note ToNote(GetNoteResponse response)
        {
            return new Note
            {
                Id = response.Id,
                Title = response.Title,
                Content = response.Content,
                CreationDate = response.CreationDate.ToLocalTime(),
                ChangeDate = response.ChangeDate.ToLocalTime(),
                IsPinned = response.IsPinned,
                Category = new Category
                {
                    Id = response.Category.Id,
                    Name = response.Category.Name,
                }
            };
        }

        private Category ToCategory(GetCategoryResponse response)
        {
            return new Category
            {
                Id= response.Id,
                Name = response.Name,
            };
        }

        private TaskItem ToTaskItem(GetTaskResponse response)
        {
            return new TaskItem
            {
                Id = response.Id,
                Content = response.Content,
                IsCompleted = response.IsCompleted,
                CreationDate = response.CreationDate.ToLocalTime(),
                Notification = response.Notification?.ToLocalTime(),
                Category = new TaskCategory
                {
                    Id = response.Category.Id,
                    Name = response.Category.Name
                }
            };
        }

        private TaskCategory ToTaskCategory(GetCategoryResponse response)
        {
            return new TaskCategory
            {
                Id = response.Id,
                Name = response.Name,
            };
        }

        private CreateNoteRequest ToCreateNoteRequest(Note note)
        {
            return new CreateNoteRequest
            {
                Title = note.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                CategoryId = note.Category.Id
            };
        }

        private UpdateNoteRequest ToUpdateNoteRequest(Note note)
        {
            return new UpdateNoteRequest
            {
                Title = note.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                CategoryId = note.Category.Id
            };
        }

        private CreateTaskRequest ToCreateTaskRequest(TaskItem task)
        {
            return new CreateTaskRequest
            {
                Content = task.Content,
                Notification = task.Notification?.ToUniversalTime(),
                CategoryId = task.Category.Id,
            };
        }

        private UpdateTaskRequest ToUpdateTaskRequest(TaskItem task)
        {
            return new UpdateTaskRequest
            {
                Content = task.Content,
                IsCompleted = task.IsCompleted,
                Notification = task.Notification?.ToUniversalTime(),
                CategoryId = task.Category.Id,
            };
        }

        private CreateCategoryRequest ToCreateCategoryRequest(Category category)
        {
            return new CreateCategoryRequest { Name = category.Name, };
        }

        private UpdateCategoryRequest ToUpdateCategoryRequest(Category category)
        {
            return new UpdateCategoryRequest { Name = category.Name, };
        }

        private CreateCategoryRequest ToCreateCategoryRequest(TaskCategory category)
        {
            return new CreateCategoryRequest { Name = category.Name, };
        }

        private UpdateCategoryRequest ToUpdateCategoryRequest(TaskCategory category)
        {
            return new UpdateCategoryRequest { Name = category.Name };
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, notesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var noteResponses = await response.Content.ReadFromJsonAsync<List<GetNoteResponse>>()
                ?? new List<GetNoteResponse>();

            return noteResponses.Select(ToNote).ToList();
        }

        public async Task<List<Category>> GetNoteCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, noteCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryReponses = await response.Content.ReadFromJsonAsync<List<GetCategoryResponse>>()
                ?? new List<GetCategoryResponse>();

            return categoryReponses.Select(ToCategory).ToList();
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, notesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            CreateNoteRequest model = ToCreateNoteRequest(note);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var noteResponse = await response.Content.ReadFromJsonAsync<GetNoteResponse>()
                ?? new GetNoteResponse();
            return ToNote(noteResponse);
        }

        public async Task UpdateNoteAsync(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, notesRoute + note.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            UpdateNoteRequest model = ToUpdateNoteRequest(note);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteNoteAsync(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, notesRoute + note.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task TogglePinNote(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Patch, notesRoute + note.Id + "/pin");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task<Category> CreateNoteCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, noteCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            CreateCategoryRequest model = ToCreateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryResponse = await response.Content.ReadFromJsonAsync<GetCategoryResponse>()
                ?? new GetCategoryResponse();
            return ToCategory(categoryResponse);
        }

        public async Task UpdateNoteCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            UpdateCategoryRequest model = ToUpdateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteNoteCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task<List<TaskItem>> GetTaskItemsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, tasksRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var taskItemResponses = await response.Content.ReadFromJsonAsync<List<GetTaskResponse>>()
                ?? new List<GetTaskResponse>();
            return taskItemResponses.Select(ToTaskItem).ToList();
        }

        public async Task<List<TaskCategory>> GetTaskCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, taskCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryResponses = await response.Content.ReadFromJsonAsync<List<GetCategoryResponse>>()
                ?? new List<GetCategoryResponse>();
            return categoryResponses.Select(ToTaskCategory).ToList();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, tasksRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            CreateTaskRequest model = ToCreateTaskRequest(task);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var taskResponse = await response.Content.ReadFromJsonAsync<GetTaskResponse>()
                ?? new GetTaskResponse();
            return ToTaskItem(taskResponse);
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, tasksRoute + task.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            UpdateTaskRequest model = ToUpdateTaskRequest(task);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, tasksRoute + task.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task<TaskCategory> CreateTaskCategoryAsync(TaskCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, taskCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            CreateCategoryRequest model = ToCreateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryResponse = await response.Content.ReadFromJsonAsync<GetCategoryResponse>()
                ?? new GetCategoryResponse();
            return ToTaskCategory(categoryResponse);
        }

        public async Task UpdateTaskCategoryAsync(TaskCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, taskCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            UpdateCategoryRequest model = ToUpdateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteTaskCategoryAsync(TaskCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }
    }
}
