using System;
using Coding.Models;

namespace Coding.Models
{
    public class FileItem:Base
    {

        public string Name { get; set; }

        public string Extension { get; set; }

        public string Content { get; set; }

        public long Size { get; set; }

        public Guid FolderId { get; set; }

        public Folder Folder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<CodeHistory> Histories { get; set; }
    }
}

