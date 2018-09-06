using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleDrive.Models
{
    public class FileDTO
    {
        public int Id { set; get; }
        public string UniqueName { set; get; }
        public string Name { set; get; }
        public int ParentFolderId { set; get; }
        public string FileExt { set; get; }
        public int FileSizeInKB { set; get; }
        public int CreatedBy { set; get; }
        public string UploadedOn { set; get; }
        public bool IsActive { set; get; }
    }
}