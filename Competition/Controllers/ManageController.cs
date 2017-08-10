using Competition.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Competition.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// 报名参加比赛
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Register(int? ID)
        {
            if (!ID.HasValue) return RedirectToAction("Index", "Home");
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c = msgEts.competition.FirstOrDefault(m => m.CompetitionID == ID.Value);
            return View();
        }

        /// <summary>
        /// 删除一场比赛
        /// </summary>
        /// <param name="ID">比赛的ID</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(int? ID)
        {
            if (ID.HasValue)
            {
                MsgBusinessLayer msgBal = new MsgBusinessLayer();
                msgBal.DeleteCompetition(ID.Value);
            }
            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// 编辑一场比赛
        /// </summary>
        /// <param name="ID">比赛的ID</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int? ID)
        {
            if (!ID.HasValue) return RedirectToAction("Index","Home");
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c=msgEts.competition.FirstOrDefault(m=>m.CompetitionID==ID.Value);
            return View(c);
        }

        /// <summary>
        /// 提交对比赛的修改
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Edit(competition c)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.RefreshCompetition(c)) return RedirectToAction("Index", "Home");
            return View(c);
        }

        /// <summary>
        /// 新建比赛
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 提交新建的比赛的信息
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Add(competition c)
        {
            MsgBusinessLayer bal = new MsgBusinessLayer();
            bal.SaveCompetition(c);
            return RedirectToAction("Index","Home");
        }
    }
}