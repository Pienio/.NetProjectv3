using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Models
{
    public class SpecializationsController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);
        // GET: Specializations
        public ActionResult Index()
        {
            if (Session["User"] is Admin)
            {
                var specs =
                db.Specializations.ToList();
                return View(specs);
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult AddSpec()
        {
            if (Session["User"] is Admin)
            {

                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DelSpec(int ID)
        {
            if (Session["User"] is Admin)
            {
                var sp = db.Specializations.Find(ID);
                var specs =
                  db.Specializations.ToList();
                if (sp.Doctors.Count != 0)
                {
                    ViewBag.MSG = "Nie możesz usunąć tej specjalizacji, istnieje doktor który ją ma";
                    return View("Index", specs);
                }
                db.BeginTransaction();
                db.Specializations.Remove(sp);
                db.Commit();
               
                return View("Index", specs);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        //  [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSpec(Specialization model, string returnUrl)
        {
            if (Session["User"] is Admin)
            {
                
                var specs =
                   db.Specializations.ToList();
                var src = db.Specializations.Select(p => p).Where(p => p.Name == model.Name);
                if (src.Count() != 0)
                {
                    ViewBag.MSG = "Istnieje już taka specjalizacja";

                    return View("Index", specs);
                }
                Specialization nowa = new Specialization();
                nowa.Name = model.Name;
                db.BeginTransaction();
                db.Specializations.Add(nowa);
                db.Commit();
                specs =
                    db.Specializations.ToList();
                return View("Index", specs);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}