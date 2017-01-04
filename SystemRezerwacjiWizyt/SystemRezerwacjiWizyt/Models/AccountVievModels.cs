using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Models
{
    public class LoginViewModel
    {

        [Required]
        [Display(Name = "PESEL")]
      
        public string Pesel { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        
    }

   
    public class EditDoctorViewModel
    {
        [Required]
        public Doctor doc { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        public string password { get; set; }

    }
}
