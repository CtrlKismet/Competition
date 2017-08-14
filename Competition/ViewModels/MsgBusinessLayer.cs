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
            return s;
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
            student s1= msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID);
            if(s1!=null)
            {
                s1.Phonenumber = s.Phonenumber;
                s1.Email = s.Email;
                s1.Gender = s.Gender;
                s1.Grade = s.Grade;
                s1.ClassID = s.ClassID;
                s1.CertainTeam = s.CertainTeam;
                /*
                 * Other Propertities is not posted to this function
                */
                msgEts.SaveChanges();
                return true;
            }
            return false;
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
            competition c= msgEts.competition.FirstOrDefault(m=>m.CompetitionID==ID);
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
        public bool RefreshCompetition(competition c)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c1 = msgEts.competition.FirstOrDefault(m => m.CompetitionID == c.CompetitionID);
            if (c1 != null)
            {
                c1.CompetitionName = c.CompetitionName;
                c1.Details = c.Details;
                c1.StartTime = c.StartTime;
                c1.EndTime = c.EndTime;
                c1.Groups = c.Groups;
                c1.TeamLimit = c.TeamLimit;
                c1.Awards = c.Awards;
                msgEts.SaveChanges();
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
            team t1 = msgEts.team.FirstOrDefault(m=>m.Member==t.Member&&m.CID==t.CID);
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
        /// <returns></returns>
        public bool DeleteTeam(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            team t = msgEts.team.FirstOrDefault(m => m.ID == ID);
            if (t == null) return false;
            msgEts.team.Remove(t);
            msgEts.SaveChanges();
            return true;
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
            return msgEts.student.FirstOrDefault(m => m.StudentID == s.StudentID && m.Password == s.Password)!=null;
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
            return s.HasPermission!=0;
        }
        #endregion

    }

}