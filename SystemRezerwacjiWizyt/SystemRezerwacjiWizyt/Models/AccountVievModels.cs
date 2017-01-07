using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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

        public List<Specialization> SpecToChoose { get; set; }

        public int SpecId { get; set; }

        public IEnumerable<string> SelectedSpec { get; set; }

    }

    public class RegisterUserVievModel
    {
        [Required]
        
        public string PESEL { get; set; }
        [Required]
        
        public string Password { get; set; }

        [Required]

        public string PasswordAgain { get; set; }
        [Required]
       
        public virtual PersonName Name { get; set; } = new PersonName();
        [Required]
      
        public DocOrPat Kind { get; set; }
        [Required]
      
        public bool Active { get; set; } = true;
        [Required]
     
        public string Mail { get; set; }
        

    }

    public class RegisterDoctorViewModel : RegisterUserVievModel
    {
        [Required]
        

        public virtual IList<Specialization> Specialization { get; set; } = new List<Specialization>();

        public List<Specialization> SpecToChoose { get; set; }
        public bool ProfileAccepted { get; set; } = false;

     
        public WorkingTime MondayWorkingTime { get; set; }

     
        public WorkingTime TuesdayWorkingTime { get; set; }
    
        public WorkingTime WednesdayWorkingTime { get; set; }
 
        public WorkingTime ThursdayWorkingTime { get; set; }

        public WorkingTime FridayWorkingTime { get; set; }

        public int SpecId { get; set; }

        public IEnumerable<SelectListItem> SelectedSpec { get; set; }
    }
}
