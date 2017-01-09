using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class SendMailToAdminViewModel
    {
        [Required]
        [Display(Name = "Twój mail")]
        public string Mail { get; set; }
        [Required]
        [Display(Name = "Temat")]
        public string Major { get; set; }

        [Required]
        [Display(Name = "Treść")]
        public string Content { get; set; }

    }
}
