using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    [DataContract(IsReference = true)]
    public class User : Entity
    {
        [Required]
        [DataMember]
        [Display(Name = "PESEL")]
        [StringLength(11, ErrorMessage = "{0} musi mieć dokładnie 11 cyfr.", MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pesel może zawierać tylko cyfry")]
        public string PESEL { get; set; }
        [Required]
        [DataMember]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        [Required]
        [DataMember]
        public virtual PersonName Name { get; set; } = new PersonName();
        [Required]
        [DataMember]
        public DocOrPat Kind { get; set; }
        [Required]
        [DataMember]
        public bool Active { get; set; } = true;
        [Required]
        [DataMember]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Wprowadź poprawny email")]
        public string Mail { get; set; }
    }
}
