using System.Configuration;
using System.Data;
using System.Windows;
using WpfNotes.Services;

namespace WpfNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var jwtToken = Settings.Default.JwtToken;

            if (!string.IsNullOrEmpty(jwtToken) && !IsJwtExpired(jwtToken))
            {
                ApiService.Initialize(jwtToken);
                new MainWindow().Show();
            }
            else
            {
                // Either no token or it's expired
                Settings.Default.JwtToken = null; // Clear expired token
                Settings.Default.Save();
                new AuthWindow().Show();
            }

            base.OnStartup(e);
        }

        private bool IsJwtExpired(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return true;
                }

                // Split the token into parts (header.payload.signature)
                var parts = jwtToken.Split('.');
                if (parts.Length != 3)
                {
                    return true; // Invalid JWT format
                }

                // Decode the payload (second part)
                var payload = parts[1];
                var jsonBytes = Convert.FromBase64String(PadBase64(payload));
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

                using var document = System.Text.Json.JsonDocument.Parse(json);
                var root = document.RootElement;

                if (root.TryGetProperty("exp", out var expElement))
                {
                    if (expElement.TryGetInt64(out long expSeconds))
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                        return DateTime.UtcNow >= expirationTime; 
                    }
                }

                // No expiration = not trusted
                return true;
            }
            catch
            {
                // If any error occurs (malformed token, etc.), assume expired
                return true;
            }
        }

        private string PadBase64(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return base64;
        }
    }

}
