using Competition.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Competition.ViewModels
{
    public class MsgBusinessLayer
    {
        #region StudentManagement

        /// <summary>
        /// 通过学号获得用户
        /// </summary>
        /// <param name="ID">用户的学号</param>
        /// <returns></returns>
        public student GetStudentByID(string ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentID == ID);
            return s==null?new student { HasPermission=0}:s;
        }

        /// <summary>
        /// 通过学号获得用户的名称
        /// </summary>
        /// <param name="ID">用户的学号</param>
        /// <returns></returns>
        public string GetUserNameByID(string ID)
        {
            if (ID == "") return "";
            student s = GetStudentByID(ID);
            return s.StudentName;
        }

        /// <summary>
        /// 新建学生信息
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public student SaveStudent(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            msgEts.student.Add(s);
            msgEts.SaveChanges();
            return s;
        }

        /// <summary>
        /// 更新学生信息
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool RefreshStudent(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s1 = msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID);
            if (s1 != null)
            {
                s1.Phonenumber = s.Phonenumber;
                s1.Email = s.Email;
                s1.Gender = s.Gender;
                s1.Grade = s.Grade;
                s1.ClassID = s.ClassID;
                s1.CertainTeam = s.CertainTeam;
                s1.RealName = s.RealName;
                if (s.Password != null) s1.Password = s.Password;
                if (s.HasPermission != null) s1.HasPermission = s.HasPermission;
                /*
                 * Other Propertities is not posted to this function
                */
                msgEts.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool ModifyStudentPassword(student s)
        {
            //s.Email里存放的是旧的用户密码
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s1 = msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID);
            if (s1 != null&&s.Email==s1.Password)
            {
                s1.Password = s.Password;
                msgEts.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测是否能报名参加这一场比赛
        /// </summary>
        /// <param name="s">需要报名的学生</param>
        /// <param name="c">报名参加的比赛</param>
        /// <returns>若不能返回错误信息，否则返回null</returns>
        public string CanRegisterToCompetition(student s, competition c)
        {
            //组别不符合
            switch (s.Grade)
            {
                case 1: if (c.Groups < 1000) { return "组别不符合！"; } break;
                case 2: if (c.Groups % 1000 < 200) { return "组别不符合！"; } break;
                case 3: if (c.Groups % 100 < 30) { return "组别不符合！"; } break;
                case 4: if (c.Groups % 10 < 4) { return "组别不符合！"; } break;
            }

            //用户已经报名参加
            List<string> teams = new List<string>();
            teams=GetMessage(s.CertainTeam);

            foreach (string id in teams)
            {
                int ID=Convert.ToInt32(id);
                team t = GetTeamByID(ID);
                if(t.CID==c.CompetitionID)
                {
                    return "已经报名参加过这场比赛！";
                }
            }
            return null;
        }

        #endregion

        #region CompetitionManagement

        /// <summary>
        /// 通过ID获得比赛
        /// </summary>
        /// <param name="ID">比赛的ID</param>
        /// <returns></returns>
        public competition GetCompetitionByID(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c = msgEts.competition.FirstOrDefault(m => m.CompetitionID == ID);
            return c;
        }

        /// <summary>
        /// 新建比赛
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public competition SaveCompetition(competition c)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            msgEts.competition.Add(c);
            msgEts.SaveChanges();
            return c;
        }

        /// <summary>
        /// 删除比赛
        /// </summary>
        /// <param name="ID">被删除比赛的ID</param>
        /// <returns></returns>
        public bool DeleteCompetition(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c = msgEts.competition.FirstOrDefault(m => m.CompetitionID == ID);
            if (c == null) return false;
            msgEts.competition.Remove(c);
            msgEts.SaveChanges();
            return true;
        }

        /// <summary>
        /// 更新比赛信息
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool RefreshCompetition(TemplateCompetition c)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c1 = msgEts.competition.FirstOrDefault(m => m.CompetitionID == c.CompetitionID);
            if (c1 != null)
            {
                c1.CompetitionName = c.CompetitionName;
                c1.Details = c.Details;
                c1.StartTime = c.StartTime;
                c1.EndTime = c.EndTime;
                c1.DeleteTime = c.DeleteTime;
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
                c1.TeamLimit = c.TeamLimit;
                msgEts.SaveChanges();
                CheckTeam(c1.CompetitionID);
                return true;
            }
            return false;
        }

        #endregion

        #region TeamManagement

        /// <summary>
        /// 通过编号获取队伍
        /// </summary>
        /// <param name="ID">队伍的编号</param>
        /// <returns></returns>
        public team GetTeamByID(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            team t = msgEts.team.FirstOrDefault(m => m.ID == ID);
            return t;
        }

        /// <summary>
        /// 确认队伍的编号
        /// </summary>
        /// <param name="ID">要查询的队伍</param>
        /// <returns></returns>
        public team GetTeamIDByTeam(team t)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            team t1 = msgEts.team.FirstOrDefault(m => m.Member == t.Member && m.CID == t.CID);
            return t1;
        }

        /// <summary>
        /// 新建队伍
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public team SaveSTeam(team t)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            msgEts.team.Add(t);
            msgEts.SaveChanges();
            return t;
        }

        /// <summary>
        /// 更新队伍信息
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool RefreshTeam(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s1 = msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID);
            if (s1 != null)
            {
                s1.Phonenumber = s.Phonenumber;
                s1.Email = s.Email;
                s1.Gender = s.Gender;
                s1.Grade = s.Grade;
                s1.ClassID = s.ClassID;
                /*
                 * Other Propertities is not posted to this function
                */
                msgEts.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除队伍
        /// </summary>
        /// <param name="ID">被删除队伍的ID</param>
        /// <param name="s1">操作的学生</param>
        /// <returns></returns>
        public bool DeleteTeam(int ID,student s1)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            team t = msgEts.team.FirstOrDefault(x => x.ID == ID);
            if (t == null) return false;

            //修改队伍里成员的CertainTeam
            List<string> teamMember = GetMessage(t.Member);

            bool first = true;

            foreach (string m in teamMember)
            {
                student s = GetStudentByID(m);

                if (first)
                {
                    if (s1.StudentID != s.StudentID&&s1.HasPermission!=100) return false;
                    first = false;
                }

                List<string> studentTeam = GetMessage(s.CertainTeam);
                string newCertainTeam = "";

                foreach (string id in studentTeam)
                {
                    if (id != ID.ToString())
                    {
                        newCertainTeam += id + "&";
                    }
                }
                
                s.CertainTeam = newCertainTeam;
                RefreshStudent(s);
            }

            msgEts.team.Remove(t);
            msgEts.SaveChanges();
            return true;
        }

        /// <summary>
        /// 检查队伍是否符合队伍信息
        /// </summary>
        /// <param name="CID"></param>
        public void CheckTeam(int CID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            List<team> allTeams = msgEts.team.ToList();
            competition c = GetCompetitionByID(CID);
            foreach (team t in allTeams)
            {
                if (t.CID==CID)
                {
                    int templateGroup = c.Groups;
                    if (t.Group % 10 > templateGroup % 10) DeleteTeam(t.ID, new student { HasPermission = 100 });
                    t.Group /= 10; templateGroup /= 10;
                    
                    if (t.Group % 10 > templateGroup % 10) DeleteTeam(t.ID, new student { HasPermission = 100 });
                    t.Group /= 10; templateGroup /= 10;

                    if (t.Group % 10 > templateGroup % 10) DeleteTeam(t.ID, new student { HasPermission = 100 });
                    t.Group /= 10; templateGroup /= 10;

                    if (t.Group % 10 > templateGroup % 10) DeleteTeam(t.ID, new student { HasPermission = 100 });
                    t.Group /= 10; templateGroup /= 10;
                }
            }
        }

        #endregion

        #region RootManagement

        /// <summary>
        ///  检验用户是否能登录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool IsValidUser(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            return msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID && m.Password == s.Password) != null;
        }

        /// <summary>
        /// 检验用户权限（通过学号）
        /// </summary>
        /// <param name="name">用户的学号</param>
        /// <returns></returns>
        public bool IsAdmin(string ID)
        {
            if (ID == "") return false;
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentID == ID);
            return s.HasPermission != 0;
        }

        /// <summary>
        /// 将所存的字符串转化为数组
        /// </summary>
        /// <param name="Member"></param>
        /// <returns></returns>
        public List<string> GetMessage(string s)
        {
            if (s == null) return new List<string>();
            List<string> msg = new List<string>();
            int len = s.Length;
            string m = "";
            for (int i = 0; i < len; i++)
            {
                if (s[i] != '&')
                {
                    m += s[i];
                }
                else
                {
                    msg.Add(m);
                    m = "";
                }
            }
            return msg;
        }
        #endregion

    }

}