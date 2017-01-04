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
    public class Doctor : Person
    {
        [Required]
        [DataMember]
        public virtual IList<Specialization> Specialization { get; set; } = new List<Specialization>();
        [DataMember]
        public bool ProfileAccepted { get; set; } = false;

        //poniższe właściwości muszą mieć w nazwie dzień tygodnia po angielsku, inaczej funkcja GetWorkingTime nie będzie działać
        [DataMember]
        public WorkingTime MondayWorkingTime { get; set; }

        [DataMember]
        public WorkingTime TuesdayWorkingTime { get; set; }
        [DataMember]
        public WorkingTime WednesdayWorkingTime { get; set; }
        [DataMember]
        public WorkingTime ThursdayWorkingTime { get; set; }
        [DataMember]
        public WorkingTime FridayWorkingTime { get; set; }

        public WorkingTime[] WeeklyWorkingTime
        {
            get
            {
                return new WorkingTime[]
                {
                    MondayWorkingTime, TuesdayWorkingTime, WednesdayWorkingTime, ThursdayWorkingTime, FridayWorkingTime
                };
            }
        }

        public void CopyFrom(Doctor doctor)
        {
           // Key = doctor.Key;
           // User.Key = doctor.User.Key;
            User.Active = doctor.User.Active;
            User.Name = doctor.User.Name;
            User.Mail = doctor.User.Mail;
            User.Password = doctor.User.Password;
            User.PESEL = doctor.User.PESEL;
            Specialization = new List<Specialization>();
            foreach (var VARIABLE in doctor.Specialization)
            {
                Specialization.Add(VARIABLE);
            }
            
            MondayWorkingTime = doctor.MondayWorkingTime;
            TuesdayWorkingTime = doctor.TuesdayWorkingTime;
            WednesdayWorkingTime = doctor.WednesdayWorkingTime;
            ThursdayWorkingTime = doctor.ThursdayWorkingTime;
            FridayWorkingTime = doctor.FridayWorkingTime;
        }

        /// <summary>
        /// Zwraca najbliższy możliwy termin wizyty z uwzględnieniem innych zaplanowanych wizyt i godzin pracy
        /// </summary>
        /// <returns></returns>
        public DateTime FirstFreeSlot
        {
            get;
           // {
                //IApplicationData db = new ApplicationDataFactory().CreateApplicationData();
                //DateTime current = NextSlot(DateTime.Now.AddMinutes(60));
                ////zmiana
                //var visits = (from v in Visits
                //              where v.Date >= current
                //              select v.Date).ToList();
                //visits.Sort((v1, v2) => DateTime.Compare(v1, v2));

                //if (visits.Count == 0 || current < visits[0])
                //{
                //    return current;
                //}
                //for (int i = 0; i < visits.Count - 1; i++)
                //{
                //    current = NextSlot(visits[i].AddMinutes(30));
                //    if (current < visits[i + 1])
                //        return current;
                //}
                //return NextSlot(visits[visits.Count - 1].AddMinutes(30));
           // }
            set; 
        } = DateTime.Now;

        /// <summary>
        /// Zwraca następny potencjalnie możliwy termin wizyty (z uwzględnieniem weekendów i godzin pracy)
        /// </summary>
        // <param name="date">Data wyjściowa</param>
        /// <returns></returns>
        //private DateTime NextSlot(DateTime date)
        //{
        //    date = date.AddMinutes(30);
        //    date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute > 30 || date.Minute == 0 ? 30 : 0, 0);
        //    WorkingTime time;
        //    do
        //    {
        //        time = GetWorkingTime(date);
        //        if (time == null || date.Hour >= time.End)
        //        {
        //            date = date.AddDays(1);
        //            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        //        }
        //        else if (date.Hour < time.Start)
        //        {
        //            return new DateTime(date.Year, date.Month, date.Day, time.Start, 0, 0);
        //        }
        //        else break;
        //    }
        //    while (true);
        //    return date;
        //}

        /// <summary>
        // Zwraca odpowiedni <see cref="WorkingTime"/> dla danego dnia tygodnia lub null w przypadku weekendu
        /// </summary>
        // <param name="date"></param>
        /// <returns></returns>
        //private WorkingTime GetWorkingTime(DateTime date)
        //{
        //    DayOfWeek day = date.DayOfWeek;
        //    return (from p in typeof(Doctor).GetProperties()
        //            where p.Name.Contains(day.ToString()) && p.PropertyType == typeof(WorkingTime)
        //            select p.GetValue(this) as WorkingTime).FirstOrDefault();
        //}

        public override string ToString() => string.Format("{0} ({1})", User.Name, Specialization);
    }
}
