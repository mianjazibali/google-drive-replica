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
                Session["Id"] = DBManager.getUserIdByLogin(dto.Login);
                
            }
            return Json(username, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Verify(string id)
        {
            int result = DBManager.verifyUser(id);
            if (result == 0)
            {
                TempData["Msg1"] = "Oops !";
                TempData["Msg2"] = " Invalid Link";
            }
            else
            {
                TempData["Msg1"] = "Well Done !";
                TempData["Msg2"] = " User Verified Successully";
            }
            return Redirect("~/User/Login");
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            int result = DBManager.resetPassword(id);
            if (result == 0)
            {
                TempData["Msg1"] = "Oops !";
                TempData["Msg2"] = " Invalid Link";
            }
            else
            {
                TempData["Msg1"] = "Reset";
                TempData["Msg2"] = id;
            }
            return Redirect("~/User/Login");
        }

        [HttpPost]
        public JsonResult ResetPassword(UserDTO dto)
        {
            return Json(DBManager.resetPassword(dto), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ForgetPassword(UserDTO dto)
        {
            return Json(DBManager.forgetPassword(dto), JsonRequestBehavior.AllowGet);
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