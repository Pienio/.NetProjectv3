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
    public class ProfileRequest : Entity
    {
        [DataMember]
        public virtual Doctor OldProfile { get; set; }
        [DataMember]
        public virtual Doctor NewProfile { get; set; }

        public ProfileRequest()
        {
        }

        public ProfileRequest(Doctor oldDoctor, Doctor newDoctor)
        {
            OldProfile = oldDoctor;
            NewProfile = newDoctor;
        }
    }
}
