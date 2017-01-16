using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Models
{
    public class RegisterVisit
    {
        public Doctor doc  {get; set;}
        public int SelectedSpec { get; set; }
        public List<Specialization> SpecToSel { get; set; }

        public List<DateTimeExt> DateToChoose { get; set; }

        public int ChosenDate { get; set; }

        public bool GetNextWeek { get; set; }

        public bool GetPasttWeek { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }
    }

    public class DateTimeExt
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public DateTime date { get; set; }

        public string See => Name + " " + date;
    }
}
