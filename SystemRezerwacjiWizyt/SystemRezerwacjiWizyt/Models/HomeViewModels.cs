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
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Wprowadź poprawny email")]
        public string Mail { get; set; }
        [Required]
        [Display(Name = "Temat")]
        public string Major { get; set; }

        [Required]
        [Display(Name = "Treść")]
        public string Content { get; set; }

    }
}
