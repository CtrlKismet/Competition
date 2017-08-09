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

        [Authorize]
        public ActionResult Edit(int? ID)
        {
            if (!ID.HasValue) return RedirectToAction("Index","Home");
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c=msgEts.competition.FirstOrDefault(m=>m.CompetitionID==ID.Value);
            return View(c);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(competition c)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            if (msgBal.RefreshCompetition(c)) return RedirectToAction("Index", "Home");
            return View(c);
        }

        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

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