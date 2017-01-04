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
        public string PESEL { get; set; }
        [Required]
        [DataMember]
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
        public string Mail { get; set; }
    }
}
