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

            if (file == null || file.ContentLength == 0) return RedirectToAction("Index");
            string fileType = "", fileSuffix = "";
            bool flag = false;
            foreach (char s in file.ContentType)
            {
                if(s=='/')
                {
                    flag = true;
                    continue;
                }
                if (flag) fileSuffix += s;
                else fileType += s;
            }
            /*
             * 文件格式错误
            if (fileType != "image") return RedirectToAction("Error", "文件格式错误");
            */
            //string newFilePath = @"F:\Program\Competition\Competition\background\";//save path   
            string newFilePath = @"C:\web\Competition\background\";//save to cloud

            /*
             * 只支持.jpg后缀的图片
             */
            file.SaveAs(newFilePath +User.Identity.Name+".jpg");//save file
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