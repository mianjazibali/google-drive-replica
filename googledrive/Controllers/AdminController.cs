using GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace googledrive.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return Redirect("~/Admin/Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string Login, string Password)
        {
            if(Login == "admin" && Password == "admin")
            {
                Session["Admin"] = "admin";
                return Json("Admin",JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult Home()
        {
            if(Session["Admin"] == null)
            {
                return Redirect("~/Admin/Login");
            }
            return View(DBManager.getLoginUsers());
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["Admin"] = null;
            Session.Abandon();
            return Redirect("~/Admin/Login");
        }
    }
}