using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    
        [DataContract]
        public class Week
        {
            [DataMember]
            public Day[] Days { get; }
            [DataMember]
            public DateTime From { get; }
            [DataMember]
            public string Title => string.Format("Aktualny tydzień: {0:dd.MM.yyyy} - {1:dd.MM.yyyy}", From, From.AddDays(6));

            public Week(DateTime monday, Day[] days)
            {
                Days = days;
                From = monday;
            }

        public static Week Create(Doctor doc, DateTime monday)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            //var doc1 = db.Doctors.Select(p => p).Where(p => p.Key == doc.Key).Include(p => p.Visits).First();
            int i = 0;
            List<Day> days = new List<Day>();
            foreach (var time in doc.WeeklyWorkingTime)
            {
                List<DateTime> slots = new List<DateTime>();
                DateTime current = monday.AddDays(i - DayOfWeekNo(monday));
                if (time != null)
                {
                    current = new DateTime(current.Year, current.Month, current.Day, time.Start, 0, 0);
                    var visits = Task.Run(() => (from v in doc.Visits
                                                 where v.Date.Year == current.Year && v.Date.Month == current.Month && v.Date.Day == current.Day
                                                 select v.Date));
                    //var visits = Task.Run(() => (from v in doc1.Visits
                    //                             where v.Date.Year == current.Year && v.Date.Month == current.Month && v.Date.Day == current.Day
                    //                            select v.Date));

                    for (DateTime s = current; s.Hour < time.End; s = s.AddMinutes(30))
                        if (!visits.Result.Contains(s) && s >= DateTime.Now.AddHours(1))
                            slots.Add(s);
                }
                if (slots.Count > 0)
                    days.Add(new Day(current.Date, slots.ToArray()));
                i++;
            }
            return new Week(monday, days.ToArray());
        }

        public static int DayOfWeekNo(DateTime day)
            {
                switch (day.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return 0;
                    case DayOfWeek.Tuesday:
                        return 1;
                    case DayOfWeek.Wednesday:
                        return 2;
                    case DayOfWeek.Thursday:
                        return 3;
                    case DayOfWeek.Friday:
                        return 4;
                }
                throw new ArgumentException("Dzień nie może być sobotą ani niedzielą.", nameof(day));
            }
        }
        [DataContract]
        public class Day
        {
            [DataMember]
            public string DayName => new CultureInfo("pl-PL").DateTimeFormat.GetDayName(Date.DayOfWeek);
            [DataMember]
            public DateTime Date { get; }
            [DataMember]
            public DateTime[] Slots { get; }

            public Day(DateTime date, DateTime[] slots)
            {
                Date = date;
                Slots = slots;
            }
        }
    
}
