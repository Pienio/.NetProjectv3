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
        [StringLength(11, ErrorMessage = "{0} musi mieć dokładnie 11 cyfr.", MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pesel może zawierać tylko cyfry")]
        public string Pesel { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        
    }

    public class EditUserVievModel
    {
        [Required]
        public  User usr  {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string password { get; set; }
    }

    public class TokenConfirmationViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Token", ErrorMessage = "Nieprawidłowy Token!")]
        public string ToWrite { get; set; }

    }

    public class RequestRefuse
    {
        [Required]
        public int RequestID { get; set; }
        [Required]
        public string Reason { get; set; }
    }
    public class EditDoctorViewModel
    {
        [Required]
        public Doctor doc { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string password { get; set; }

        public List<Specialization> SpecToChoose { get; set; }
        
        public int? SpecId { get; set; }

        public IEnumerable<string> SelectedSpec { get; set; }

    }

    public class RegisterUserVievModel
    {
        [Required]
        [Display(Name = "PESEL")]
        [StringLength(11, ErrorMessage = "{0} musi mieć dokładnie 11 cyfr.", MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pesel może zawierać tylko cyfry")]
        public string PESEL { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Hasła się nie zgadzają, wprowadź je ponownie!")]
        [Display(Name = "Powtórz hasło")]
        public string PasswordAgain { get; set; }
        [Required]
       
        public  PersonName Name { get; set; } = new PersonName();


        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Wprowadź poprawny email")]
        public string Mail { get; set; }
        

    }

    public class RegisterDoctorViewModel : RegisterUserVievModel
    {
        [Required]
        public Doctor doc { get; set; }
        
        public List<Specialization> SpecToChoose { get; set; }

        public int? SpecId { get; set; }

        public IEnumerable<string> SelectedSpec { get; set; }
    }
}
