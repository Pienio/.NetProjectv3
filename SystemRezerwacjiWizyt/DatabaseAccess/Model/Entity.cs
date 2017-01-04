using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace DatabaseAccess.Model
{
    [DataContract(IsReference = true)]
    public class Entity
    { 
        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        //[Required]
        [DataMember]
        [Timestamp]
        public byte[] Version { get; set; } 
    }
}
