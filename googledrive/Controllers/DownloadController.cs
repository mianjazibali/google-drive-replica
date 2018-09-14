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
            if(Session["Login"] == null)
            {
                Session["Login"] = "Login";
            }
            FileDTO dto = DBManager.getFile(id, (string)Session["Login"]);
            if (dto.UniqueName == null)
            {
                ViewBag.Msg = "Not Found / Access Denied";
                return View("NotExist");
            }
            else
            {
                ViewData["Name"] = dto.Name;
                ViewData["FileExt"] = dto.FileExt;
                ViewData["FileSizeInKB"] = dto.FileSizeInKB;
                ViewData["UniqueName"] = dto.UniqueName;
                return View();
            } 
        }

        [HttpPost]
        public JsonResult UpdateDownloadCount(string UniqueName)
        {
            return Json(DBManager.updateDownloadCount(UniqueName), JsonRequestBehavior.AllowGet);
        }
    }
}