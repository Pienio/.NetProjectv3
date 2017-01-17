using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SystemRezerwacjiWizyt.Models;
using DatabaseAccess;
using DatabaseAccess.Model;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using MailService;

namespace SystemRezerwacjiWizyt.Controllers
{
    //  [Authorize]
    public class AccountController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        //  [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var pass = SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.Password);
            var a = db.Users.Select(p => p).Where(p => p.PESEL == model.Pesel && p.Password == pass && p.Active);
            if (a.Count() == 0)
            {
                ViewBag.Error = "Nieprawidłowy PESEL lub hasło.";
                return View();
            }
            else
            {
                var c = a.First();

                if (c.Kind == DocOrPat.Doctor)
                {
                    var docc = db.Doctors.First(p => p.User.Key == c.Key);
                    if (docc.ProfileAccepted)
                        Session["User"] = db.Doctors.First(p => p.User.Key == c.Key);
                    else
                        ViewBag.Error = "Twoje konto nie zostało jeszcze zaakceptowane przez administratora.";
                }
                else if (c.Kind == DocOrPat.Patient)
                {
                    Session["User"] = db.Patients.First(p => p.User.Key == c.Key);
                }
                else
                {
                    Session["User"] = db.Admins.First(p => p.User.Key == c.Key);
                }
                //SignInAsync(a.First().Name.ToString());
                //Session["User"] = c;

            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            return RedirectToAction("Index", "Home", new { error = ViewBag.Error });

        }

        //[AllowAnonymous]
        public ActionResult LogOut(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Requests(string returnUrl)
        {
            if (Session["User"] is Admin)
            {
                List<ProfileRequest> lista = new List<ProfileRequest>();
                lista = db.Requests.Select(p => p).ToList();
                return View(lista);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AcceptRequests(int ID, string returnUrl)
        {
            if (Session["User"] is Admin)
            {
                List<ProfileRequest> lista = new List<ProfileRequest>();
                MailServices tosend = new MailServices();
                var a = db.Requests.Find(ID);
                if (a.OldProfile == null)
                {
                    db.BeginTransaction();
                    var doc = db.Doctors.Find(a.NewProfile.Key);
                    doc.ProfileAccepted = true;
                    db.Requests.Remove(a);
                    db.Commit();
                    tosend.SendAcceptationMail(doc.User.Mail);
                    lista = db.Requests.Select(p => p).ToList();
                    return View("Requests", lista);

                }
                db.BeginTransaction();
                var docold = db.Doctors.Find(a.OldProfile.Key);
                var docnew = db.Doctors.Find(a.NewProfile.Key);
                docold.CopyFrom(docnew);
                docold.ProfileAccepted = true;
                db.Users.Remove(docnew.User);
                db.Doctors.Remove(docnew);
                db.Requests.Remove(a);
                db.Commit();
                tosend.SendAcceptationEditMail(docold.User.Mail);
                lista = db.Requests.Select(p => p).ToList();
                return View("Requests", lista);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeclineRequests(int ID, string returnUrl)
        {
            if (Session["User"] is Admin)
            {
                //List<ProfileRequest> lista = new List<ProfileRequest>();
                //var a = db.Requests.Find(ID);
                //if (a.OldProfile == null)
                //{
                //    db.BeginTransaction();
                //    var doc = db.Doctors.Find(a.NewProfile.Key);
                //    db.Doctors.Remove(doc);
                //    db.Requests.Remove(a);
                //    db.Commit();

                //    lista = db.Requests.Select(p => p).ToList();
                //    return View("Requests", lista);

                //}
                //db.BeginTransaction();
                //var docold = db.Doctors.Find(a.OldProfile.Key);
                //var docnew = db.Doctors.Find(a.NewProfile.Key);
                //docold.ProfileAccepted = true;
                //db.Doctors.Remove(docnew);
                //db.Requests.Remove(a);
                //db.Commit();
                //lista = db.Requests.Select(p => p).ToList();
                //return View("Requests", lista);
                RequestRefuse nn = new RequestRefuse();
                nn.RequestID = ID;
                Session["RequestRefuse"] = nn;
                return RedirectToAction("DeclineRequestsReason");
                //return View("DeclineRequestsReason",nn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DeclineRequestsReason(string returnUrl)
        {
            RequestRefuse nn = Session["RequestRefuse"] as RequestRefuse;
            ;
            return View(nn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeclineRequestsReason(RequestRefuse model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (Session["User"] is Admin)
            {
                if (model.Reason == null)
                    model.Reason = "";
                List<ProfileRequest> lista = new List<ProfileRequest>();
                MailServices tosend = new MailServices();
                var a = db.Requests.Find(model.RequestID);
                if (a.OldProfile == null)
                {
                    db.BeginTransaction();
                    var doc = db.Doctors.Find(a.NewProfile.Key);
                    string mail = doc.User.Mail;
                    db.Users.Remove(doc.User);
                    db.Doctors.Remove(doc);
                    db.Requests.Remove(a);
                    db.Commit();

                    tosend.SendRejectionMail(mail, model.Reason);
                    lista = db.Requests.Select(p => p).ToList();
                    ViewBag.Message = "Pomyślnie odrzucono wniosek.";
                    return View("Requests", lista);

                }
                db.BeginTransaction();
                var docold = db.Doctors.Find(a.OldProfile.Key);
                var docnew = db.Doctors.Find(a.NewProfile.Key);
                docold.ProfileAccepted = true;
                db.Users.Remove(docnew.User);
                db.Doctors.Remove(docnew);
                
                db.Requests.Remove(a);
                db.Commit();
                // MailServices tosend = new MailServices();
                tosend.SendEditRejectionMail(docold.User.Mail, model.Reason);
                lista = db.Requests.Select(p => p).ToList();
                ViewBag.Message = "Pomyślnie odrzucono wniosek.";
                return View("Requests", lista);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult EditPatient(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Patient a = Session["User"] as Patient;
            EditUserVievModel b = new EditUserVievModel();
            b.usr = new User();
            b.usr = a.User;


            return View(b);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPatient(EditUserVievModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }
            ViewBag.ReturnUrl = returnUrl;
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Patient a = Session["User"] as Patient;
            if (a.User.Password != Utils.PasswordHasher.CreateHash(model.password))
            {
                EditUserVievModel b = new EditUserVievModel();
                b.usr = new User();
                b.usr = a.User;
                ViewBag.Error = "Niepoprawne hasło.";
                return View(b);
            }
            db.BeginTransaction();
            var pat = db.Patients.Find(a.Key);
            pat.User.Name = model.usr.Name;
            pat.User.Mail = model.usr.Mail;

            //pat.User.Password = a.User.Password;
            db.Commit();


            return RedirectToAction("Index", "Home", new { message="Zapisano zmiany." });
        }

        public ActionResult EditDoctor(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Doctor a = Session["User"] as Doctor;
            EditDoctorViewModel b = new EditDoctorViewModel();
            b.doc = new Doctor();
            b.password = "";
            b.doc.CopyFrom(a);
            b.SpecToChoose = db.Specializations.Select(p => p).ToList();

            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDoctor(EditDoctorViewModel model, string returnUrl, string Del)
        {
            model.SpecToChoose = db.Specializations.Select(p => p).ToList();


            ViewBag.ReturnUrl = returnUrl;
            //  model.SpecToChoose = db.Specializations.Select(p => p).ToList();
            List<Specialization> nowaa = new List<Specialization>();
            foreach (var specialization in model.doc.Specialization)
            {
                var sdf = db.Specializations.Select(p => p).First(p => p.Name == specialization.Name);
                nowaa.Add(sdf);
            }
            if (model.SpecId != null && model.SpecId != 0)
            {
                ModelState.Clear();
                Specialization toad = db.Specializations.Select(p => p).Where(p => p.Key == model.SpecId).First();

                var abc = model.doc.Specialization.Select(p => p).Where(p => p.Name == toad.Name);

                if (abc.Count() == 0)
                {

                    // Specialization toad = db.Specializations.Select(p => p).Where(p => p.Key == model.SpecId).First();
                    nowaa.Add((toad));
                    model.doc.Specialization = nowaa;

                }
                else
                {
                    model.doc.Specialization = nowaa;
                    ViewBag.Message = "Nie możesz dodać dwóch takich samych specjalizacji.";
                }


                // model.SpecToChoose = db.Specializations.Select(p => p).ToList();
                model.password = "";
                model.SpecId = 0;
                //Request.Form["SpecId"] = "";
                return View(model);
                //return RedirectToAction("EditDoctor", "Account", model);

            }
            if (model.SelectedSpec != null)
            {
                ModelState.Clear();
                if (model.SelectedSpec.Count() != model.doc.Specialization.Count)
                {


                    foreach (var VARIABLE in model.SelectedSpec)
                    {
                        int key = Int32.Parse(VARIABLE);
                        var todel = model.SpecToChoose[key-1];
                        var todel1 = nowaa.Select(p => p).Where(p => p.Name == todel.Name).First();
                        nowaa.Remove(todel1);
                    }
                }





                model.doc.Specialization = nowaa;

                model.password = "";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Doctor a = Session["User"] as Doctor;
            if (string.IsNullOrEmpty(model.password) ||
                a.User.Password != SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.password))
            {
                ViewBag.Error = "Błędne hasło.";
                return View(model);
                //return RedirectToAction("Index", "Home");
            }

            db.BeginTransaction();
            var k = db.Doctors.Select(p => p).First(p => p.Key == a.Key);
            k.ProfileAccepted = false;
            model.doc.ProfileAccepted = false;
            model.doc.Specialization = nowaa;
            //db.Doctors.Add(model.doc);
            ProfileRequest nowy = new ProfileRequest();
            nowy.OldProfile = k;
            nowy.NewProfile = model.doc;
            db.Requests.Add(nowy);
            db.Commit();
            Session["User"] = null;

            //Tu sie dobrze edytowało
            //db.BeginTransaction();
            //var k = db.Doctors.Select(p=>p).First(p => p.Key==a.Key);

            //k.CopyFrom(model.doc);

            //k.Specialization = nowaa;
            ////db.SetEntryToModified(k);
            ////  var manager = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager;

            //// manager.ChangeObjectState(k, EntityState.Modified);
            ////db.SetEntryToModified(k);
            //Session["User"] = model.doc;
            //db.Commit();

            return RedirectToAction("Index", "Home", new {message = "Wysłano prośbę o edycję konta do administratora. O jej rozpatrzeniu zostaniesz powiadomiony mailowo, po czym będziesz mógł zalogować się na swoje konto."});
        }

        public ActionResult RegisterPatient(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult RegisterDoctor(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var b = new RegisterDoctorViewModel();
            b.doc = new Doctor();
            b.doc.ProfileAccepted = false;
            b.doc.User = new User();
            b.doc.User.Name = new PersonName();
            b.doc.Specialization = new List<Specialization>();
            b.SpecToChoose = db.Specializations.Select(p => p).ToList();
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPatient(RegisterUserVievModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ViewBag.ReturnUrl = returnUrl;
            var a = db.Users.Select(p => p).Where(p => p.PESEL == model.PESEL && p.Active);
            if (a.Count() != 0)
            {
                ViewBag.Error = "Użytkownik o podanym Peselu już istnieje";
                return View(model);
            }
            // db.BeginTransaction();
            Patient d = new Patient();
            d.User = new User();
            d.User.Name = model.Name;
            d.User.Kind = DocOrPat.Patient;
            d.User.Mail = model.Mail;
            d.User.PESEL = model.PESEL;
            d.User.Password = SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.Password);
            //  db.Patients.Add(d);
            // db.Commit();
            //  var usr = db.Patients.Select(p => p).First(p => p.User.PESEL == model.PESEL);
            // Session["User"] = usr;
            Session["TMP"] = d;
            return RedirectToAction("RegisterPatientToken", "Account");
        }

        public ActionResult RegisterPatientToken(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            TokenConfirmationViewModel b = new TokenConfirmationViewModel();
            b.Token = Utils.Token.GetToken();
            b.ToWrite = "";
            var Patient = Session["TMP"] as Patient;
            MailServices tosend = new MailServices();
            tosend.SendRegistrationConfirmation(Patient.User.Mail, b.Token);
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPatientToken(TokenConfirmationViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //if (model.Token != model.ToWrite)
            //{
            //    return View(model);
            //}
            var Patient = Session["TMP"] as Patient;
            db.BeginTransaction();
            db.Patients.Add(Patient);
            db.Commit();
            var usr = db.Patients.Select(p => p).First(p => p.User.PESEL == Patient.User.PESEL);
            Session["User"] = usr;
            MailServices tosend = new MailServices();
            //tosend.SendAcceptationMail(usr.User.Mail);
            return RedirectToAction("Index", "Home", new {message = "Rejestracja przebiegła pomyślnie."});
        }

        public ActionResult RegisterDoctorToken(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            TokenConfirmationViewModel b = new TokenConfirmationViewModel();
            b.Token = Utils.Token.GetToken();
            b.ToWrite = "";
            var Doctor = Session["TMP"] as Doctor;
            MailServices tosend = new MailServices();
            tosend.SendRegistrationConfirmation(Doctor.User.Mail, b.Token);
            return View(b);
        }
        public ActionResult ChangePassword(string returnUrl)
        {

            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(SystemRezerwacjiWizyt.Models.ChangePassword model, string returnUrl)
        {
            if (!ModelState.IsValid || Session["User"] == null)
            {
                return View();
            }
            var a = Session["User"] as Person;
            if (Utils.PasswordHasher.CreateHash(model.password) == a.User.Password)
            {
                
                db.BeginTransaction();
                var usr = db.Users.Find(a.User.Key);
                usr.Password = Utils.PasswordHasher.CreateHash(model.newpass);
                db.Commit();
                if (a.User.Kind == DocOrPat.Doctor)
                {
                    Session["User"] = db.Doctors.Select(p => p).First(p => p.User.Key == a.User.Key);
                }
                else
                {
                    Session["User"] = db.Patients.Select(p => p).First(p => p.User.Key == a.User.Key);
                }
                return RedirectToAction("Index", "Home", new { message = "Hasło zmienione pomyślnie." });
            }
            ViewBag.Error = "Stare hasło jest nieprawidłowe.";
            return View();
        }

        public ActionResult DeleteAccount(string returnUrl)
        {
            
            return View();
        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAccount(AccountDelete model, string returnUrl)
        {
            if (!ModelState.IsValid||Session["User"] == null)
            {
                return View();
            }

            var a = Session["User"] as Person;
            if (Utils.PasswordHasher.CreateHash(model.password) == a.User.Password)
            {
                if (a.User.Kind == DocOrPat.Doctor)
                {
                    //var doc = db.Doctors.Select(p => p).First(p => p.User.Key == a.User.Key);
                    //db.BeginTransaction();
                    //foreach (var q in doc.Visits)
                    //{
                    //    var patt = q.Patient;
                    //    var patt1 = db.Patients.Find(q.Key);
                    //    patt1.Visits.Remove(q);
                    //    db.Visits.Remove(q);
                    //}
                    //db.Doctors.Remove(doc);
                    //db.Commit();
                    db.BeginTransaction();
                    IEnumerable<Visit> obw = db.Visits.Select(p => p).Where(p => p.Doctor.User.Key == a.User.Key);
                    var doc = db.Doctors.Select(p => p).First(p => p.User.Key == a.User.Key);
                    if (obw == null || obw.ToList().Count == 0)
                    {
                        
                      
                        db.Doctors.Remove(doc);

                    }
                    else
                    {
                        int ind = 0;
                        for (int i = 0; i < doc.Visits.Count; i++)
                        {
                            if (doc.Visits[i].Date > DateTime.Now)
                            {
                               
                                db.Visits.Remove(doc.Visits[i]);
                                ind++;
                            }
                        }
                        //obw = db.Visits.Select(p => p).Where(p => p.Doctor.User.Key == a.User.Key);
                        if (ind==doc.Visits.Count)
                        {


                            db.Doctors.Remove(doc);

                        }
                        else
                        {
                            doc.User.Active = false;
                        }
                    }
                    db.Commit();

                }
                else
                {
                    //var pat = db.Patients.Select(p => p).First(p => p.User.Key == a.User.Key);
                    //db.BeginTransaction();
                    //foreach (var q in pat.Visits)
                    //{
                    //    var docc = q.Doctor;
                    //    var docc1 = db.Doctors.Find(q.Key);
                    //    docc1.Visits.Remove(q);
                    //    db.Visits.Remove(q);
                    //}
                    //db.Patients.Remove(pat);
                    //db.Commit();

                    db.BeginTransaction();
                    IEnumerable<Visit> obw = db.Visits.Select(p => p).Where(p => p.Patient.User.Key == a.User.Key);
                    var pat = db.Patients.Select(p => p).First(p => p.User.Key == a.User.Key);
                    if (obw == null || obw.ToList().Count == 0)
                    {


                        db.Patients.Remove(pat);

                    }
                    else
                    {
                        int ind = 0;
                        for (int i = 0; i < pat.Visits.Count; i++)
                        {
                            if (pat.Visits[i].Date > DateTime.Now)
                            {
                                ind++;
                                db.Visits.Remove(pat.Visits[i]);

                            }
                        }
                        //obw = db.Visits.Select(p => p).Where(p => p.Patient.User.Key == a.User.Key);
                        if (ind==pat.Visits.Count)
                        {


                            db.Patients.Remove(pat);

                        }
                        else
                        {
                            pat.User.Active = false;
                        }

                    }
                    db.Commit();
                }
                Session["User"] = null;
            }
            return RedirectToAction("Index", "Home", new { message = "Konto zostało usunięte z systemu." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDoctorToken(TokenConfirmationViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //if (model.Token != model.ToWrite)
            //{
            //    return View(model);
            //}
            var Doctor = Session["TMP"] as Doctor;
            ProfileRequest now = new ProfileRequest();
            Doctor.ProfileAccepted = false;

            now.NewProfile = Doctor;

            db.BeginTransaction();
            // db.Doctors.Add(Doctor);
            db.Requests.Add(now);
            //db.Doctors.Add(Doctor);
            db.Commit();
            //var usr = db.Doctors.Select(p => p).First(p => p.User.PESEL == Doctor.User.PESEL);
            //Session["User"] = usr;
            //MailServices tosend = new MailServices();
            //tosend.SendAcceptationMail(usr.User.Mail);
            return RedirectToAction("Index", "Home", new { message = "Twoja prośba o akceptację profilu została przesłana do administratora. O jej rozpatrzeniu zostaniesz powiadomiony mailowo." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDoctor(RegisterDoctorViewModel model, string returnUrl)
        {
            model.SpecToChoose = db.Specializations.Select(p => p).ToList();

            ViewBag.ReturnUrl = returnUrl;

            List<Specialization> nowaa = new List<Specialization>();
            foreach (var specialization in model.doc.Specialization)
            {
                var sdf = db.Specializations.Select(p => p).First(p => p.Name == specialization.Name);
                nowaa.Add(sdf);
            }
        
            if (model.SpecId != null && model.SpecId != 0)
            {
                ModelState.Clear();
                Specialization toad = db.Specializations.Select(p => p).Where(p => p.Key == model.SpecId).First();

                var abc = model.doc.Specialization.Select(p => p).Where(p => p.Name == toad.Name);

                if (abc.Count() == 0)
                {

                    // Specialization toad = db.Specializations.Select(p => p).Where(p => p.Key == model.SpecId).First();
                    nowaa.Add((toad));
                    model.doc.Specialization = nowaa;

                }
                else
                {
                    model.doc.Specialization = nowaa;
                    ViewBag.Message = "Nie możesz dodać dwóch takich samych specjalizacji.";
                }


                // model.SpecToChoose = db.Specializations.Select(p => p).ToList();

                model.SpecId = 0;
                //Request.Form["SpecId"] = "";
                return View(model);
                //return RedirectToAction("EditDoctor", "Account", model);

            }
            if (model.SelectedSpec != null)
            {
                ModelState.Clear();
                if (model.SelectedSpec.Count() != model.doc.Specialization.Count)
                {


                    foreach (var VARIABLE in model.SelectedSpec)
                    {
                        int key = Int32.Parse(VARIABLE);
                        var todel = model.SpecToChoose[key-1];//db.Specializations.Find(key);
                        var todel1 = nowaa.Select(p => p).Where(p => p.Name == todel.Name).First();
                        nowaa.Remove(todel1);
                    }
                }


               

                //model.doc.Specialization.Clear();
                model.doc.Specialization = nowaa;


                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var a = db.Users.Select(p => p).Where(p => p.PESEL == model.PESEL&&p.Active);
            if (a.Count() != 0)
            {
                ViewBag.Error = "Użytkownik o podanym PESEL-u już istnieje.";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }



            //  db.BeginTransaction();
            Doctor d = new Doctor();
            d.User = new User();
            d.User.Name = model.Name;
            d.User.Kind = DocOrPat.Doctor;
            d.User.Mail = model.Mail;
            d.User.PESEL = model.PESEL;
            d.User.Password = SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.Password);
            d.Specialization= new List<Specialization>();
            foreach (var spc in model.doc.Specialization)
            {
                var dpc = db.Specializations.Select(p => p).Where(p => spc.Name == p.Name).First();
                d.Specialization.Add(dpc);
            }
           // d.Specialization = model.doc.Specialization;
            d.FridayWorkingTime = model.doc.FridayWorkingTime;
            d.MondayWorkingTime = model.doc.MondayWorkingTime;
            d.ThursdayWorkingTime = model.doc.ThursdayWorkingTime;
            d.TuesdayWorkingTime = model.doc.TuesdayWorkingTime;
            d.WednesdayWorkingTime = model.doc.WednesdayWorkingTime;
            d.ProfileAccepted = false;
            //  db.Doctors.Add(d);
            //   db.Commit();
            //    var usr = db.Doctors.Select(p => p).First(p => p.User.PESEL == model.PESEL);
            //   Session["User"] = usr;
            // return RedirectToAction("Index", "Home");
            Session["TMP"] = d;
            return RedirectToAction("RegisterDoctorToken", "Account");
        }


        public ViewResult BlankEditorRow(string idd)
        {
            // string spectoadd = SelectedSpec; //Request.Form["SelectedSpec"].ToString();
            // int c = Int32.Parse(idd);
            //// int c = id;
            // Specialization toadd = db.Specializations.Select(p => p).Where(p => p.Key == c).First();
            Specialization d = new Specialization();
            d.Name = "pupa";
            return View("_Specializations", d);
        }

        //private void SignInAsync(string name)
        //{
        //    var claims = new List<Claim>();
        //    claims.Add(new Claim(ClaimTypes.Name,name));   
        //    var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        //    var ctx = Request.GetOwinContext();
        //    var authenticationManager = ctx.Authentication;
        //    authenticationManager.SignIn(id);
        //}

        public ActionResult PasswordReset()
        {
            PasswordResetViewModel a = new PasswordResetViewModel();
            a.maill = "";
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordReset(PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            db.BeginTransaction();
            var a = db.Users.Select(p => p).Where(p => p.Mail == model.maill);
            User b = null;
            if (a.Count() != 0)
                b = a.First();


            if (b != null)
            {
                string newPasssword = Utils.Token.GetToken();
                b.Password = Utils.PasswordHasher.CreateHash(newPasssword);
                MailServices tosend = new MailServices();
                tosend.SendPasswordResetMail(b.Mail, newPasssword);
                db.Commit();
                return RedirectToAction("Index", "Home", new { message= " Nowe hasło zostało wysłane na podany adres mailowy. Użyj go do zalogowania się i za jego pomocą zmień hasło." });

            }
            db.Commit();
            ViewBag.Error = "Nie istnieje użytkownik o takim mailu.";
            return View(new PasswordResetViewModel());
        }
    }
}