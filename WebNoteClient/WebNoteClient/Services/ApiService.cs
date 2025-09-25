using NuGet.Common;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebNoteClient.ApiModels.Category;
using WebNoteClient.ApiModels.Note;
using WebNoteClient.ApiModels.TaskItem;
using WebNoteClient.Models.Note;
using WebNoteClient.Models.TaskItem;

namespace WebNoteClient.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        private readonly string notesRoute = "api/notes/";
        private readonly string noteCategoriesRoute = "api/categories/";
        private readonly string tasksRoute = "api/tasks/";
        private readonly string taskCategoriesRoute = "api/taskcategories/";

        public ApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        #region NoteMappers
        private NoteModel ToNote(GetNoteResponse response)
        {
            return new NoteModel
            {
                Id = response.Id,
                Title = response.Title,
                Content = response.Content,
                CreationDate = response.CreationDate,
                ChangeDate = response.ChangeDate,
                IsPinned = response.IsPinned,
                Category = new NoteCategoryModel
                {
                    Id = response.Category.Id,
                    Name = response.Category.Name,
                }
            };
        }

        private CreateNoteRequest ToCreateNoteRequest(NoteModel note)
        {
            return new CreateNoteRequest
            {
                Title = note.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                CategoryId = note.Category.Id
            };
        }

        private UpdateNoteRequest ToUpdateNoteRequest(NoteModel note)
        {
            return new UpdateNoteRequest
            {
                Title = note.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                CategoryId = note.Category.Id
            };
        }

        #endregion

        #region TaskMappers
        private TaskItemModel ToTaskItem(GetTaskResponse response)
        {
            return new TaskItemModel
            {
                Id = response.Id,
                Content = response.Content,
                IsCompleted = response.IsCompleted,
                CreationDate = response.CreationDate.ToLocalTime(),
                Notification = response.Notification?.ToLocalTime(),
                Category = new TaskCategoryModel
                {
                    Id = response.Category.Id,
                    Name = response.Category.Name
                }
            };
        }

        private CreateTaskRequest ToCreateTaskRequest(TaskItemModel task)
        {
            return new CreateTaskRequest
            {
                Content = task.Content,
                Notification = task.Notification?.ToUniversalTime(),
                CategoryId = task.Category.Id,
            };
        }

        private UpdateTaskRequest ToUpdateTaskRequest(TaskItemModel task)
        {
            return new UpdateTaskRequest
            {
                Content = task.Content,
                IsCompleted = task.IsCompleted,
                Notification = task.Notification?.ToUniversalTime(),
                CategoryId = task.Category.Id,
            };
        }
        #endregion

        #region CategoryMappers
        private NoteCategoryModel ToCategory(GetCategoryResponse response)
        {
            return new NoteCategoryModel
            { 
                Id = response.Id, 
                Name = response.Name 
            };
        }

        private TaskCategoryModel ToTaskCategory(GetCategoryResponse response)
        {
            return new TaskCategoryModel
            {
                Id = response.Id,
                Name = response.Name,
            };
        }

        private CreateCategoryRequest ToCreateCategoryRequest(NoteCategoryModel category)
        {
            return new CreateCategoryRequest { Name = category.Name, };
        }

        private CreateCategoryRequest ToCreateCategoryRequest(TaskCategoryModel category)
        {
            return new CreateCategoryRequest { Name = category.Name, };
        }

        private UpdateCategoryRequest ToUpdateCategoryRequest(NoteCategoryModel category)
        {
            return new UpdateCategoryRequest { Name = category.Name, };
        }

        private UpdateCategoryRequest ToUpdateCategoryRequest(TaskCategoryModel category)
        {
            return new UpdateCategoryRequest { Name = category.Name, };
        }
        
        #endregion

        #region Notes
        public async Task<List<NoteModel>> GetNotesAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, notesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var noteResponses = await response.Content.ReadFromJsonAsync<List<GetNoteResponse>>()
                ?? new List<GetNoteResponse>();

            return noteResponses.Select(ToNote).ToList();
        }

        public async Task<NoteModel> GetNoteAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, notesRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var noteReponse = await response.Content.ReadFromJsonAsync<GetNoteResponse>()
                ?? new GetNoteResponse();
            return ToNote(noteReponse);
        }

        public async Task<NoteModel> CreateNoteAsync(string token, NoteModel note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, notesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        public async Task UpdateNoteAsync(string token, NoteModel note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, notesRoute + note.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UpdateNoteRequest model = ToUpdateNoteRequest(note);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteNoteAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, notesRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task ToggleNotePinAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, notesRoute + id + "/pin/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        #endregion

        #region NoteCategories
        public async Task<List<NoteCategoryModel>> GetNoteCategoriesAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, noteCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryReponses = await response.Content.ReadFromJsonAsync<List<GetCategoryResponse>>()
                ?? new List<GetCategoryResponse>();

            return categoryReponses.Select(ToCategory).ToList();
        }

        public async Task<NoteCategoryModel> CreateNoteCategoryAsync(string token, NoteCategoryModel category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, noteCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        public async Task UpdateNoteCategoryAsync(string token, NoteCategoryModel category)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, noteCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UpdateCategoryRequest model = ToUpdateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteNoteCategoryAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, noteCategoriesRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        #endregion

        #region Tasks
        public async Task<List<TaskItemModel>> GetTasksAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, tasksRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
            var taskItemResponses = await response.Content.ReadFromJsonAsync<List<GetTaskResponse>>()
                ?? new List<GetTaskResponse>();
            return taskItemResponses.Select(ToTaskItem).ToList();
        }

        public async Task<TaskItemModel> GetTaskItemAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, tasksRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
            var taskItemResponse = await response.Content.ReadFromJsonAsync<GetTaskResponse>()
                ?? new GetTaskResponse();
            return ToTaskItem(taskItemResponse);
        }

        public async Task<TaskItemModel> CreateTaskItemAsync(string token, TaskItemModel task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, tasksRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        public async Task UpdateTaskItemAsync(string token, TaskItemModel task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var request = new HttpRequestMessage(HttpMethod.Put, tasksRoute + task.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UpdateTaskRequest model = ToUpdateTaskRequest(task);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteTaskItemAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, tasksRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task ToggleTaskCompleteAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, tasksRoute + id + "/complete/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        #endregion

        #region TaskCategories

        public async Task<List<TaskCategoryModel>> GetTaskCategoriesAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, taskCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }

            var categoryResponses = await response.Content.ReadFromJsonAsync<List<GetCategoryResponse>>()
                ?? new List<GetCategoryResponse>();
            return categoryResponses.Select(ToTaskCategory).ToList();
        }

        public async Task<TaskCategoryModel> CreateTaskCategoryAsync(string token, TaskCategoryModel category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, taskCategoriesRoute);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

        public async Task UpdateTaskCategoryAsync(string token, TaskCategoryModel category)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, taskCategoriesRoute + category.Id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UpdateCategoryRequest model = ToUpdateCategoryRequest(category);
            request.Content = JsonContent.Create(model);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        public async Task DeleteTaskCategoryAsync(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, taskCategoriesRoute + id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
            }
        }

        #endregion
    }
}
