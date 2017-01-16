using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiWizyt.Models;
using DatabaseAccess;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Controllers
{
    public class VisitController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);
        // GET: Visit
        public ActionResult AddVisit(int DoctorId)
        {
            var a = db.Doctors.Find(DoctorId);
            RegisterVisit nowa = new RegisterVisit();
            nowa.doc = a;
            nowa.SpecToSel = a.Specialization.ToList();

            var first = a.FirstFreeSlot;
            first = first.AddDays(-Week.DayOfWeekNo(first));
            var CurrentWeek =  Week.Create(a, first);
            Session["Week"] = CurrentWeek;
            int i = 1;
            List<DateTimeExt> listadat = new List<DateTimeExt>();
            foreach (var day in CurrentWeek.Days)
            {
                foreach (var date in day.Slots)
                {
                    DateTimeExt n= new DateTimeExt();
                    n.Key = i++;
                    n.Name = day.DayName;
                    n.date = new DateTime(date.Year,date.Month,date.Day,date.Hour,date.Minute,date.Second);
                    listadat.Add(n);
                }
                
            }
            nowa.DateToChoose = listadat;
            return View(nowa);
        }
        [HttpPost]       
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddVisit(RegisterVisit model)
        {
            Doctor a = db.Doctors.Find(model.doc.Key);
            if (model.GetNextWeek)
            {
                Week cur = Session["Week"] as Week;
               
                var CurrentWeek = Week.Create(a, cur.From.AddDays(7));
                Session["Week"] = CurrentWeek;
                int i = 1;
                List<DateTimeExt> listadat = new List<DateTimeExt>();
                foreach (var day in CurrentWeek.Days)
                {
                    foreach (var date in day.Slots)
                    {
                        DateTimeExt n = new DateTimeExt();
                        n.Key = i;
                        n.Name = day.DayName;
                        n.date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        listadat.Add(n);
                        i++;
                    }

                }
               
                model.doc = a;
                model.SpecToSel = a.Specialization.ToList();
                model.DateToChoose = listadat;
                return View(model);
            }
            if (model.GetPasttWeek)
            {
                Week cur = Session["Week"] as Week;
               
                var CurrentWeek = Week.Create(a, cur.From.AddDays((-7)));
                Session["Week"] = CurrentWeek;
                int i = 1;
                List<DateTimeExt> listadat = new List<DateTimeExt>();
                foreach (var day in CurrentWeek.Days)
                {
                    foreach (var date in day.Slots)
                    {
                        DateTimeExt n = new DateTimeExt();
                        n.Key = i;
                        n.Name = day.DayName;
                        n.date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                        listadat.Add(n);
                        i++;
                    }

                }
                
                model.doc = a;
                model.SpecToSel = a.Specialization.ToList();
                model.DateToChoose = listadat;
                return View(model);
            }

            db.BeginTransaction();
            Patient pat = Session["User"] as Patient;
            Visit toAdd = new Visit(pat, a, model.DateToChoose[model.ChosenDate - 1].date,
                a.Specialization[model.SelectedSpec - 1]);
                a.Visits.Add(toAdd);
                pat.Visits.Add(toAdd);

            db.Visits.Add(toAdd);
            db.Commit();
            MailService.MailServices tosend = new MailService.MailServices();
            tosend.SendVisitRegistrationNotifications(a,pat, model.DateToChoose[model.ChosenDate - 1].date);
            ViewBag.Message = "Wizyta została zarejestrowana";
            return RedirectToAction("Index", "Home", new { message = $"Wizyta u {a.User.Name} w terminie {toAdd.Date:g} została pomyślnie zarezerwowana." });
        }

        public ActionResult DeleteConfirmationVisit(int VisitId)
        {
            return View(VisitId);
        }
        public ActionResult DeleteVisit(int VisitId)
        {
            var a = db.Visits.Find(VisitId);
            var b = db.Doctors.Find(a.Doctor.Key);
            var c = db.Patients.Find(a.Patient.Key);
            Person crt = Session["User"] as Person;
            db.BeginTransaction();
            b.Visits.Remove(a);
            c.Visits.Remove(a);
            db.Visits.Remove(a);
            db.Commit();
            MailService.MailServices tosend= new MailService.MailServices();
            tosend.SendVisitDeleteNotification(a,crt.User.Kind);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ShowVisits()
        {
            if (Session["User"] is Doctor)
            {
                Doctor p = Session["User"] as Doctor;
                List<Visit> lista = p.Visits.ToList();
                return View(lista);
            }
            else
            {
                Patient pat = Session["User"] as Patient;
                List<Visit> lista = pat.Visits.ToList();
                return View(lista);
            }
           
        }
    }
}