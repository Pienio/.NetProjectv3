using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    [DataContract]
    public class Specialization : Entity
    {
        [Required]
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public IList<Doctor> Doctors { get; set; } = new List<Doctor>();

        public Specialization() : base() { }
        public Specialization(string name) : base()
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
