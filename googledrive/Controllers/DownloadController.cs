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
            ViewData["Name"] = dto.Name;
            ViewData["File"] = dto.UniqueName + dto.FileExt;
            return View();
        }
    }
}