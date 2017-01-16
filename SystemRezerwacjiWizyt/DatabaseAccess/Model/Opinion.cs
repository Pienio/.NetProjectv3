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
    public class Opinion : Entity
    {
        [DataMember]
        [Required]
        public Doctor TargetDoctor { get; set; }

        [DataMember]
        //[Required]
        public Patient Author { get; set; }

        [DataMember]
        public int Rate { get; set; }

        [DataMember]
        [Required]
        public string AuthorNick { get; set; }
        
        [DataMember]
        public string Description { get; set; }
    }
}
