using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DatabaseAccess.Model;


namespace UnitTestProject1
{
    public static class TestingExtension
    {
       

        public static Patient GetPatient()
        {

            Patient p = new Patient();
            
            p.User = new User()
            {
                Kind = DocOrPat.Patient,
                Name = new PersonName() { Name = "F", Surname = "M" },
                Password = "96e79218965eb72c92a549dd5a330112",
                PESEL = "11111111111",
                Active = true,
                Mail = "lold@gmail.com"
                
            };
           
            return p;

        }

        public static Doctor GetDoctor()
        {
           
                var spec = new Specialization();
                spec.Name = "Okulista";
                Doctor g = new Doctor() { User = new User() };
                g.Specialization =new Specialization[] { spec };
                g.User.Name.Name = "Jan";
                g.User.Name.Surname = "Janowski";
                g.ProfileAccepted = true;
                 g.User.Mail = "lololo@gmail.com";
                g.User.PESEL = "77777777777";
                g.User.Kind = DocOrPat.Doctor;
                g.User.Password = "96e79218965eb72c92a549dd5a330112";
                g.MondayWorkingTime = new WorkingTime() { Start = 8, End = 12 };
                g.TuesdayWorkingTime = new WorkingTime() { Start = 8, End = 12 };
                g.WednesdayWorkingTime = new WorkingTime() { Start = 8, End = 12 };
                g.ThursdayWorkingTime = new WorkingTime() { Start = 8, End = 12 };
                g.FridayWorkingTime = new WorkingTime() { Start = 8, End = 12 };
                return g;
           

        }

    }

    
}
