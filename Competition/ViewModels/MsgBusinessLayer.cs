using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Competition.ViewModels
{
    public class MsgBusinessLayer
    {
        /// <summary>
        /// 保存学生信息
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
                /*
                 * Other Propertities is not posted to this function
                */
                msgEts.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存比赛
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
                msgEts.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        ///  检验用户是否能登录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool IsValidUser(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            return msgEts.student.FirstOrDefault(m => m.StudentName == s.StudentName && m.Password == s.Password)!=null;
        }

        /// <summary>
        /// 检验用户权限
        /// </summary>
        /// <param name="name">用户的学号（注意name和ID是被交换的）</param>
        /// <returns></returns>
        public bool IsAdmin(string name)
        {
            if (name == "") return false;
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentName == name);
            return s.HasPermission!=0;
        }

        /// <summary>
        /// 得到用户的名称
        /// </summary>
        /// <param name="name">用户的学号（注意name和ID是被交换的）</param>
        /// <returns></returns>
        public string GetUserName(string name)
        {
            if (name == "") return "";
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentName == name);
            return s.StudentID;
        }
    }
    
}