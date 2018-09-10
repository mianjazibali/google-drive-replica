using GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoogleDrive.Controllers
{
    public class UserController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return Redirect("~/User/Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ValidateUser(UserDTO dto)
        {
            string username = DBManager.GetUser(dto);
            if (username != "")
            {
                Session["Username"] = username;
                Session["Login"] = dto.Login;
            }
            return Json(username, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegisterUser(UserDTO dto)
        {
            return Json(DBManager.registerUser(dto), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session["Login"] = null;
            Session.Abandon();
            return Redirect("~/User/Login");
        }
    }
}