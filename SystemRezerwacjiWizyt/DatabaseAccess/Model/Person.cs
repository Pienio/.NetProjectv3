using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    /// <summary>
    /// Tutaj będą lądować wspólne metody i właściwości dla Doctor i Patient by wywoływać je z GUI
    /// </summary>
    
    [DataContract(IsReference = true)]
    public abstract class Person : Entity
    {
        [Required]
        [DataMember]
        public virtual User User { get; set; }
        [DataMember]
        public virtual IList<Visit> Visits { get; set; } = new List<Visit>();
    }
}
