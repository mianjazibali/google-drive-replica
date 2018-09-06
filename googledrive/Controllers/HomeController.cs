using GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoogleDrive.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Login"] == null)
            {
                return Redirect("~/User/Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ListFolders(int Id)
        {
            return Json(DBManager.getFoldersList(Id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListFiles(int Id)
        {
            return Json(DBManager.getFilesList(Id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateFolder(FolderDTO dto)
        {
            string login = (string)Session["Login"];
            dto.Id = DBManager.getUserIdByLogin(login);
            return Json(DBManager.createFolder(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles(FileDTO dto)
        {
            int parent = dto.Id;
            string login = (string)Session["Login"];
            int id = DBManager.getUserIdByLogin(login);
            if (Request.Files.Count > 0)
            {
                List<FileDTO> dtos = new List<FileDTO>();
                try
                {
                    string uniqueName = "";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        dto = new FileDTO();
                        HttpPostedFileBase file = files[i];
                        dto.Name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                        var ext = System.IO.Path.GetExtension(file.FileName);
                        dto.FileSizeInKB = file.ContentLength / 1024;
                        dto.FileExt = ext;
                        dto.UniqueName = Guid.NewGuid().ToString();
                        uniqueName = dto.UniqueName + ext;
                        dto.ParentFolderId = parent;
                        dto.CreatedBy = id;
                        var rootPath = Server.MapPath("~/Uploads");
                        var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);
                        file.SaveAs(fileSavePath);
                        dtos.Add(new FileDTO()
                        {
                            Name = dto.Name,
                            UniqueName = dto.UniqueName,
                            ParentFolderId = dto.ParentFolderId,
                            FileExt = dto.FileExt,
                            FileSizeInKB = dto.FileSizeInKB,
                            CreatedBy = dto.CreatedBy
                        });
                    }
                    int result = DBManager.uploadFiles(dtos);
                    return Json(result + " File(s) Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}