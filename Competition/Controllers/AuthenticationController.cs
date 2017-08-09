using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Competition.ViewModels;
using System.Web.Mvc;
using System.Web.Security;

namespace Competition.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(student s)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.IsValidUser(s))
            {
                FormsAuthentication.SetAuthCookie(s.StudentName, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CredentialError", "Invalid Username or Password");
                return View("Login");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(student s)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();

            s.HasPermission = 0;
            msgBal.SaveStudent(s);

            return RedirectToAction("Login");
        }
    }
}