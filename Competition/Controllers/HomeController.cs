using Competition.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Competition.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 网站首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            ViewCompetition c = new ViewCompetition();
            c.Competitions = msgEts.competition.ToList();
            c.HasPermission = msgBal.IsAdmin(User.Identity.Name);
            return View(c);
        }

        /// <summary>
        /// 显示用户
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowUsers()
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            List<student> s = msgEts.student.ToList();
            TemplateStudents t = new TemplateStudents();
            t.tittle = "所有用户"; t.students = s;
            return View(t);
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public ActionResult SearchUser(string keyword)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            List<student> s = msgEts.student.ToList();
            List<student> s1 = new List<student>();
            foreach (student templateStudent in s)
            {
                if (templateStudent.StudentID.ToLower().Contains(keyword.ToLower()) || templateStudent.StudentName.ToLower().Contains(keyword.ToLower()))
                {
                    s1.Add(templateStudent);
                }
            }
            TemplateStudents t = new TemplateStudents();
            t.tittle = "用户 \"" + keyword + "\" 搜索结果"; t.students = s1;
            return View("ShowUsers", t);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDetails(string ID)
        {
            if (ID == null)
            {
                return View("Home");
            }
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentID == ID);
            if (s == null)
            {
                return View("Home");
            }
            return View(s);
        }

        /// <summary>
        /// 用户设置
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult UserSettings(string ID)
        {
            //判断是否有权限
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.GetStudentByID(User.Identity.Name).HasPermission == 0&&User.Identity.Name!=msgBal.GetStudentByID(ID).StudentID)
            {
                return RedirectToAction("Index", "Home");
            }

            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentID == ID);
            return View(s);
        }

        /// <summary>
        /// 用户信息修改
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult UserSettings(student s)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.RefreshStudent(s)) return RedirectToAction("UserDetails", new { ID = s.StudentID });
            return View(s);
        }

        /// <summary>
        /// 上传图片至服务器
        /// </summary>
        /// <param name="file">图片文件</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            //图片大小<=2MB=2097152Byte
            if (file == null || file.ContentLength == 0 || file.ContentLength >= 2097152) return View("~/Views/Shared/Error.cshtml", new ErrorMessage { Url = "/Home/UserSettings?ID="+User.Identity.Name, ErrorMsg = "未上传图片或者图片超过2MB！" });
            string fileType = "", fileSuffix = "";
            bool flag = false;
            foreach (char s in file.ContentType)
            {
                if (s == '/')
                {
                    flag = true;
                    continue;
                }
                if (flag) fileSuffix += s;
                else fileType += s;
            }
            //文件格式错误
            if (fileType != "image") return View("~/Views/Shared/Error.cshtml", new ErrorMessage { Url="/Home/Index",ErrorMsg="文件格式错误！"});
            
            //string newFilePath = @"F:\Program\Competition\Competition\background\";//save path  
            string newFilePath = @"C:\web\Competition\background\";//save to cloud

            //删除原有的图片
            string tmp = Directory.EnumerateFiles(newFilePath, $"{User.Identity.Name}.*").First();
            System.IO.File.Delete(tmp);

            //保存新的图片
            ViewBag.imagesource = newFilePath + User.Identity.Name + "." + fileSuffix;
            file.SaveAs(ViewBag.imagesource);//save file
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 查看比赛详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        //[Authorize]
        public ActionResult ViewCompetition(int? ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            if (!ID.HasValue) return RedirectToAction("Index");
            competition c = msgEts.competition.SingleOrDefault(m => m.CompetitionID == ID);
            return View(c);
        }
    }

    public class TemplateStudents
    {
        public string tittle { get; set; }
        public List<student> students { get; set; }
    }
}

namespace Competition
{
    public class ErrorMessage
    {
        public string Url { get; set; }
        public string ErrorMsg { get; set; }
    }
}