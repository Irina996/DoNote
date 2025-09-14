using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNoteClient.ApiModels.Auth
{
    public class AuthResponse
    {
        public string token { get; set; }
        public string username { get; set; }

        public DateTime expires { get; set; }
    }
}
