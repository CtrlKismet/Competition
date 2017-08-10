using Competition.ViewModels;
using System;
using System.Collections.Generic;
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
            c.Competitions= msgEts.competition.ToList();
            c.HasPermission = msgBal.IsAdmin(User.Identity.Name);
            return View(c);
        }

        /// <summary>
        /// 用户设置
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult UserSettings()
        {
            string userName = User.Identity.Name;
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s= msgEts.student.FirstOrDefault(m => m.StudentName == userName);
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
            if (msgBal.RefreshStudent(s)) return RedirectToAction("Index");
            return View(s);
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
            competition c = msgEts.competition.SingleOrDefault(m=>m.CompetitionID==ID);
            return View(c);
        }
    }
}