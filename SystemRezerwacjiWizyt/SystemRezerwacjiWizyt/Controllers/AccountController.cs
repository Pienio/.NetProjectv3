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
                ViewBag.Error = "Nieprawidłowy Pesel lub hasło";
            }
            else
            {
                var c = a.First();

                if (c.Kind == DocOrPat.Doctor)
                {
                    Session["User"] = db.Doctors.First(p => p.User.Key == c.Key);
                }    
                else
                {
                    Session["User"] = db.Patients.First(p => p.User.Key == c.Key);
                }
                //SignInAsync(a.First().Name.ToString());
                //Session["User"] = c;

            }
            
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            return RedirectToAction("Index", "Home");

        }

        //[AllowAnonymous]
        public ActionResult LogOut(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EditDoctor(string returnUrl)
        {
           
            ViewBag.ReturnUrl = returnUrl;
            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Doctor a= Session["User"] as Doctor;
            EditDoctorViewModel b = new EditDoctorViewModel();
            b.doc = new Doctor();
            b.password = "";
            b.doc.CopyFrom(a);
            b.SpecToChoose = db.Specializations.Select(p => p).ToList();

            return View(b);
        }
       
        [HttpPost]     
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDoctor(EditDoctorViewModel model, string returnUrl,string Del )
        {

            ViewBag.ReturnUrl = returnUrl;
            model.SpecToChoose = db.Specializations.Select(p => p).ToList();
            List<Specialization> nowaa = new List<Specialization>();
            foreach (var specialization in model.doc.Specialization)
            {
                var sdf = db.Specializations.Select(p => p).First(p => p.Name == specialization.Name);
                nowaa.Add(sdf);
            }
            if (model.SpecId != 0)
            {
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
                    ViewBag.Mess = "Nie możesz dodać dwóch takich samych specjalizacji";
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
                if (model.SelectedSpec.Count() != model.doc.Specialization.Count)
                {
                    
               
                foreach (var VARIABLE in model.SelectedSpec)
                {
                    int key = Int32.Parse(VARIABLE);
                    var todel = db.Specializations.Find(key);
                    var todel1 = nowaa.Select(p => p).Where(p => p.Name == todel.Name).First();
                    nowaa.Remove(todel1);
                }
            }


               
               
                
                model.doc.Specialization = nowaa;

                model.password = "";
                return View(model);
            }

            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");
            Doctor a = Session["User"] as Doctor;
            if (string.IsNullOrEmpty(model.password)||a.User.Password != SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.password))
            {
                ViewBag.Message = "Błędne hasło";
                return View(model);
                //return RedirectToAction("Index", "Home");
            }
            db.BeginTransaction();
            var k = db.Doctors.Select(p=>p).First(p => p.Key==a.Key);
       
            k.CopyFrom(model.doc);
           
            k.Specialization = nowaa;
            //db.SetEntryToModified(k);
            //  var manager = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager;

            // manager.ChangeObjectState(k, EntityState.Modified);
            //db.SetEntryToModified(k);
            Session["User"] = model.doc;
            db.Commit();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
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
            b.doc= new Doctor();
            b.doc.ProfileAccepted = false;
            b.doc.User= new User();
            b.doc.User.Name=new PersonName();
            b.doc.Specialization= new List<Specialization>();
            b.SpecToChoose = db.Specializations.Select(p => p).ToList();
            return View(b);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPatient(RegisterUserVievModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            db.BeginTransaction();
            Patient d= new Patient();
            d.User=new User();
            d.User.Name = model.Name;
            d.User.Kind=DocOrPat.Patient;
            d.User.Mail = model.Mail;
            d.User.PESEL = model.PESEL;
            d.User.Password = SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.Password);
            db.Patients.Add(d);
            db.Commit();
            var usr = db.Patients.Select(p => p).First(p => p.User.PESEL == model.PESEL);
            Session["User"] = usr;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDoctor(RegisterDoctorViewModel model, string returnUrl )
        {
            ViewBag.ReturnUrl = returnUrl;
            model.SpecToChoose = db.Specializations.Select(p => p).ToList();
            List<Specialization> nowaa = new List<Specialization>();
            foreach (var specialization in model.doc.Specialization)
            {
                var sdf = db.Specializations.Select(p => p).First(p => p.Name == specialization.Name);
                nowaa.Add(sdf);
            }
            if (model.SpecId != 0)
            {
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
                    ViewBag.Mess = "Nie możesz dodać dwóch takich samych specjalizacji";
                }


                // model.SpecToChoose = db.Specializations.Select(p => p).ToList();
                
                model.SpecId = 0;
                //Request.Form["SpecId"] = "";
                return View(model);
                //return RedirectToAction("EditDoctor", "Account", model);

            }
            if (model.SelectedSpec != null)
            {
                if (model.SelectedSpec.Count() != model.doc.Specialization.Count)
                {


                    foreach (var VARIABLE in model.SelectedSpec)
                    {
                        int key = Int32.Parse(VARIABLE);
                        var todel = db.Specializations.Find(key);
                        var todel1 = nowaa.Select(p => p).Where(p => p.Name == todel.Name).First();
                        nowaa.Remove(todel1);
                    }
                }





                model.doc.Specialization = nowaa;

               
                return View(model);
            }





            if (model.Password == model.PasswordAgain)
            {
                
            
            db.BeginTransaction();
            Doctor d = new Doctor();
            d.User = new User();
            d.User.Name = model.Name;
            d.User.Kind = DocOrPat.Doctor;
            d.User.Mail = model.Mail;
            d.User.PESEL = model.PESEL;
            d.User.Password = SystemRezerwacjiWizyt.Utils.PasswordHasher.CreateHash(model.Password);
            d.Specialization = model.doc.Specialization;
            d.FridayWorkingTime = model.doc.FridayWorkingTime;
            d.MondayWorkingTime = model.doc.MondayWorkingTime;
            d.ThursdayWorkingTime = model.doc.ThursdayWorkingTime;
            d.TuesdayWorkingTime = model.doc.TuesdayWorkingTime;
            d.WednesdayWorkingTime = model.doc.WednesdayWorkingTime;
            d.ProfileAccepted = true;
            db.Doctors.Add(d);
            db.Commit();
            var usr = db.Doctors.Select(p => p).First(p => p.User.PESEL == model.PESEL);
            Session["User"] = usr;
            return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Mess = "Hasła muszą być takie same";
                return View(model);
            }
            
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


    }
}