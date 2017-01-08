using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Models
{
    public class IndexHomeViewModels
    {
        public List<Doctor> docs { get; set; }
        public List<Specialization> specs { get; set; }

        public int? SelSPec { get; set; }

        public string Text { get; set; }
    }
}
