using System.Net.Http.Headers;
using WebNoteClient.ApiModels.Category;
using WebNoteClient.ApiModels.Note;
using WebNoteClient.Models.Note;

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

        private NoteCategoryModel ToCategory(GetCategoryResponse response)
        {
            return new NoteCategoryModel
            { 
                Id = response.Id, 
                Name = response.Name 
            };
        }

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

        public async Task<NoteModel> GetNote(string token, int id)
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
    }
}
