using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebNoteClient.ApiModels.Category;

namespace WebNoteClient.ApiModels.Note
{
    public class GetNoteResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public bool IsPinned { get; set; } = false;

        public GetCategoryResponse Category { get; set; }
    }
}
