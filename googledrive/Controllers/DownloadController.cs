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
            if ((dto.UniqueName == null) || (dto.UniqueName == ""))
            {
                return View("NotExist");
            }
            ViewData["Name"] = dto.Name;
            ViewData["FileExt"] = dto.FileExt;
            ViewData["FileSizeInKB"] = dto.FileSizeInKB;
            ViewData["File"] = dto.UniqueName + dto.FileExt;
            return View();
        }
    }
}