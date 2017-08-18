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

            //已经超过报名截止时间
            if (System.DateTime.Now.Date > c.EndTime.Date)
            {
                tmp.ErrorMsg = "已经超过报名时间！";
                return View("ErrorRegister", tmp);
            }

            team t = new team();

            List<student> templateStudents = new List<student>();
            bool[] vis = new bool[5];
            int group = 0, count = 0;

            //将表格信息转化为队伍信息
            foreach (string m in tmp.member)
            {
                if (m == "" || m == null) continue;
                t.Member += m + "&";
                student s = msgBal.GetStudentByID(m);
                if (s == null)
                {
                    tmp.ErrorMsg = "用户 \"" + m + "\" 不存在！";
                    return View("ErrorRegister", tmp);
                }

                string errorMsg = msgBal.CanRegisterToCompetition(s, c);
                if (errorMsg != null)
                {
                    tmp.ErrorMsg = "用户 \"" + m +"\" "+ errorMsg;
                    return View("ErrorRegister", tmp);
                }

                if(templateStudents.FirstOrDefault(x=>x.StudentID==s.StudentID)!=null)
                {
                    tmp.ErrorMsg = "用户 \"" + m + "\" 重复填写！";
                    return View("ErrorRegister", tmp);
                }
                templateStudents.Add(s);
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
                count++;
            }
            t.CID = tmp.number;
            t.Group = group;
            t.Number = count;

            if (t.Number > c.TeamLimit)
            {
                tmp.ErrorMsg = "参赛人数超过限制！";
                return View("ErrorRegister", tmp);
            }

            //保存队伍
            msgBal.SaveSTeam(t);
            t = msgBal.GetTeamIDByTeam(t);

            //更新队员信息
            foreach (string m in tmp.member)
            {
                if (m == "" || m == null) continue;
                student s = msgBal.GetStudentByID(m);
                student s1 = s; s1.CertainTeam += t.ID + "&";
                msgBal.RefreshStudent(s1);
            }

            return RedirectToAction("UserDetails", "Home", new { ID = User.Identity.Name });
        }


        /// <summary>
        /// 查看队伍
        /// </summary>
        /// <param name="ID">比赛的ID</param>
        /// <returns></returns>
        public ActionResult ViewTeams(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            List<team> t = msgEts.team.ToList();
            List<team> t1 = new List<team>();
            foreach (team Team in t)
            {
                if(Team.CID==ID)
                {
                    t1.Add(Team);
                }
            }
            return View("ViewTeams",t1);
        }

        /// <summary>
        /// 删除一支队伍
        /// </summary>
        /// <param name="ID">队伍的ID</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteTeam(int? ID)
        {
            if (ID.HasValue)
            {
                MsgBusinessLayer msgBal = new MsgBusinessLayer();
                student s = msgBal.GetStudentByID(User.Identity.Name);
                if(s.HasPermission!=0)
                {
                    msgBal.DeleteTeam(ID.Value);
                }
            }
            return RedirectToAction("UserDetails", "Home", new { ID = User.Identity.Name });
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
                student s = msgBal.GetStudentByID(User.Identity.Name);
                if (s.HasPermission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
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
            student s = msgBal.GetStudentByID(User.Identity.Name);
            if (s.HasPermission == 0)
            {
                return RedirectToAction("Index", "Home");
            }
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
        public ActionResult Edit(TemplateCompetition c)
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
            MsgBusinessLayer msgBal = new MsgBusinessLayer();
            student s = msgBal.GetStudentByID(User.Identity.Name);
            if (s.HasPermission == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// 提交新建的比赛的信息
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Add(TemplateCompetition c)
        {
            MsgBusinessLayer bal = new MsgBusinessLayer();
            competition c1 = new competition();
            c1.CompetitionName = c.CompetitionName;
            c1.Details = c.Details;
            c1.StartTime = c.StartTime;
            c1.EndTime = c.EndTime;
            c1.TeamLimit = c.TeamLimit;
            c1.Groups = 0;
            foreach (int group in c.grade)
            {
                switch (group)
                {
                    case 1: c1.Groups += 1000; break;
                    case 2: c1.Groups += 200; break;
                    case 3: c1.Groups += 30; break;
                    case 4: c1.Groups += 4; break;
                }
            }
            bal.SaveCompetition(c1);
            return RedirectToAction("Index", "Home");
        }
    }

    public class TemplateTeam
    {
        /// <summary>
        /// 比赛ID
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 成员学号
        /// </summary>
        public string[] member { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class TemplateCompetition
    {
        public int CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public string Details { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TeamLimit { get; set; }
        /// <summary>
        /// 复选框中参赛组别
        /// </summary>
        public int[] grade { get; set; }
    }
}