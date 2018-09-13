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
            int UserId = (int)Session["Id"];
            return Json(DBManager.getFoldersList(Id, UserId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchFolders(string search)
        {
            int UserId = (int)Session["Id"];
            return Json(DBManager.searchFolders(search, UserId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchFiles(string search)
        {
            int UserId = (int)Session["Id"];
            return Json(DBManager.searchFiles(search, UserId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GenerateFileToken(int id)
        {
            return Json(DBManager.generateFileToken(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveFileToken(int id)
        {
            return Json(DBManager.removeFileToken(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GenerateSpecificFileToken(int Id, string Login)
        {
            return Json(DBManager.generateSpecificFileToken(Id, Login), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ListFiles(int Id)
        {
            int UserId = (int)Session["Id"];
            return Json(DBManager.getFilesList(Id, UserId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateFolder(FolderDTO dto)
        {
            string login = (string)Session["Login"];
            dto.CreatedBy = DBManager.getUserIdByLogin(login);
            return Json(DBManager.createFolder(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditFile(FileDTO dto)
        {
            return Json(DBManager.editFile(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFileUsers(int id)
        {
            return Json(DBManager.getFileUsers(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSharedUser(int Id, string Login)
        {
            return Json(DBManager.deleteSharedUser(Id, Login), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFolder(FolderDTO dto)
        {
            return Json(DBManager.deleteFolder(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFile(FileDTO dto)
        {
            return Json(DBManager.deleteFile(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DownloadFile(int id)
        {
            return Json(DBManager.downloadFile(id), JsonRequestBehavior.AllowGet);

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
                    return Json(result);
                    
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