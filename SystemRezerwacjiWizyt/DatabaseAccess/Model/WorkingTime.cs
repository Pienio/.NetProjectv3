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
    /// <summary>
    /// Zakładamy, że godziny dotyczą tego samego dnia, tj Start < End
    /// </summary>
    [ComplexType]
    [DataContract]
    public class WorkingTime
    {
        [DataMember]
        [Range(0, 24)]
        public int Start { get; set; }
        [DataMember]
        [Range(0, 24)]
        public int End { get; set; }

        public WorkingTime() 
        { 

        }
    }
}
