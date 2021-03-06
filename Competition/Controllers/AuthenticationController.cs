﻿using System;
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
            if(User.Identity.Name!=null&&User.Identity.Name!="")
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorMessage { ErrorMsg = "您已成功登录！", Url = "/Home/Index" });
            }
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(student s)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.IsValidUser(s))
            {
                FormsAuthentication.SetAuthCookie(s.StudentID, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CredentialError", "学号或密码错误");
                return View("Login");
            }
        }

        /// <summary>
        /// 下拉窗口登录验证
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubLogin(student s)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.IsValidUser(s))
            {
                FormsAuthentication.SetAuthCookie(s.StudentID, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CredentialError", "学号或密码错误");
                return View("Login");
                //return RedirectToAction("Login", "Authentication");
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View("Register");
        }

        /// <summary>
        /// 注册新用户 Post
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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