namespace WebNoteClient.Models.Auth
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }

        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime Expires { get; set; } = DateTime.MinValue;
    }
}
