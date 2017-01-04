using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    [ComplexType]
    [DataContract]
    public class PersonName
    {
        [Required]
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        [Required]
        public virtual string Surname { get; set; }

        public override string ToString()
        {
            return Name + ' ' + Surname;
        }
    }
}
