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
            return RedirectToAction("Index", "Home");
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
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            competition c = msgBal.GetCompetitionByID(ID.Value);
            return View(c);
        }

        /// <summary>
        /// 验证队伍信息
        /// </summary>
        /// <param name="tmp">队伍人员</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Register(TemplateTeam tmp)
        {
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            competition c = msgBal.GetCompetitionByID(tmp.number);
            team t = new team();
            bool[] vis = new bool[5];
            int group = 0;

            //将表格信息转化为队伍信息
            foreach (string m in tmp.member)
            {
                t.Member += m + "&";
                student s = msgBal.GetStudentByID(m);
                if (s == null)
                {
                    tmp.ErrorMsg = "输入的用户不存在！";
                    return View("ErrorRegister", tmp);
                }
                if (!vis[s.Grade])
                {
                    switch (s.Grade)
                    {
                        case 1: group += 1000; break;
                        case 2: group += 200; break;
                        case 3: group += 30; break;
                        case 4: group += 4; break;
                    }
                    vis[s.Grade] = true;
                }
            }
            t.CID = tmp.number;
            t.Group = group;
            t.Number = tmp.member.Length;

            bool flag = true;
            if (group>c.Groups|| group%1000 > c.Groups%1000 || group % 100 > c.Groups % 100 || group % 10 > c.Groups % 10)
            {
                flag = false;
            }

            if (!flag || t.Number > c.TeamLimit)
            {
                tmp.ErrorMsg = "输入的用户不符合参赛要求！";
                return View("ErrorRegister", tmp);
            }

            //保存队伍
            msgBal.SaveSTeam(t);
            t = msgBal.GetTeamIDByTeam(t);
            //更新队员信息
            foreach (string m in tmp.member)
            {
                student s = msgBal.GetStudentByID(m);
                student s1 = s;s1.CertainTeam += t.ID + "&";
                msgBal.RefreshStudent(s1);
            }

            return RedirectToAction("UserDetails", "Home",new { ID = User.Identity.Name });
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
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 编辑一场比赛
        /// </summary>
        /// <param name="ID">比赛的ID</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int? ID)
        {
            if (!ID.HasValue) return RedirectToAction("Index", "Home");
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            competition c = msgBal.GetCompetitionByID(ID.Value);
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
            return RedirectToAction("Index", "Home");
        }
    }

    public class TemplateTeam
    {
        public int number { get; set; }
        public string[] member { get; set; }
        public string ErrorMsg { get; set; }
    }
}