using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Competition.ViewModels
{
    public class ViewCompetition
    {
        public bool HasPermission { get; set; }
        public List<competition> Competitions { get; set; }
    }
}