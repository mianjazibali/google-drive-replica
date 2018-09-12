using GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace googledrive.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            return Redirect("~/Download/File");
        }

        public ActionResult File(string id)
        {
            if(id == null)
            {
                return Redirect("~/User/Login");
            }
            FileDTO dto = DBManager.getFile(id);
            if ((dto.UniqueName == null) && (dto.Share == null))
            {
                ViewBag.Msg = "File Not Found";
                return View("NotExist");
            }
            else
            if ((dto.UniqueName != null) && (dto.Share != null)){
                if((string)Session["Login"] == dto.Share)
                {
                    ViewData["Name"] = dto.Name;
                    ViewData["FileExt"] = dto.FileExt;
                    ViewData["FileSizeInKB"] = dto.FileSizeInKB;
                    ViewData["File"] = dto.UniqueName + dto.FileExt;
                    return View();
                }
                ViewBag.Msg = "Access Denied";
                return View("NotExist");
            }
            else
            {
                ViewData["Name"] = dto.Name;
                ViewData["FileExt"] = dto.FileExt;
                ViewData["FileSizeInKB"] = dto.FileSizeInKB;
                ViewData["File"] = dto.UniqueName + dto.FileExt;
                return View();
            } 
        }
    }
}