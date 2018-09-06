using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleDrive.Models
{
    public class FolderDTO
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public int ParentFolderId { set; get; }
        public int CreatedBy { set; get; }
        public string CreatedOn { set; get; }
        public bool IsActive { set; get; }
    }
}