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
    public class Visit : Entity
    {
      //  [Required]
        [DataMember]
        public virtual Patient Patient { get; set; }
        [Required]
        [DataMember]
        public virtual Doctor Doctor { get; set; }
        [Required]
        [DataMember]
        public DateTime Date { get; set; } =DateTime.Today;

        public Visit() { }

        public Visit(Patient patient, Doctor doctor, DateTime date)
        {
            Patient = patient;
            Doctor = doctor;
            Date = date;
        }
    }
}
