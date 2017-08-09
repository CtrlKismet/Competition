using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Competition.ViewModels
{
    public class MsgBusinessLayer
    {
        public student SaveStudent(student s)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            msgEts.student.Add(s);
            msgEts.SaveChanges();
            return s;
        }

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

        public competition SaveCompetition(competition c)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            msgEts.competition.Add(c);
            msgEts.SaveChanges();
            return c;
        }

        public bool DeleteCompetition(int ID)
        {
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            competition c= msgEts.competition.FirstOrDefault(m=>m.CompetitionID==ID);
            if (c == null) return false;
            msgEts.competition.Remove(c);
            msgEts.SaveChanges();
            return true;
        }

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

        public bool IsAdmin(string name)
        {
            if (name == "") return false;
            totalmsgdbEntities msgEts = new totalmsgdbEntities();
            student s = msgEts.student.FirstOrDefault(m => m.StudentName == name);
            return s.HasPermission!=0;
        }
    }
    
}