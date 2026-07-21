using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coding.Models
{
    public class Folder:Base
    {
        public string Name { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        public Guid? ParentFolderId { get; set; }

        public Folder? ParentFolder { get; set; }

        public ICollection<Folder> ChildFolders { get; set; }

        public ICollection<FileItem> Files { get; set; }
    }
}

