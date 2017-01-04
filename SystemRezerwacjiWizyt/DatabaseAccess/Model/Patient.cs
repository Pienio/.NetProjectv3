using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    [DataContract(IsReference = true)]
    public class Patient : Person
    {
        public override string ToString()
        {
            return User.Name.ToString();
        }
    }
}
