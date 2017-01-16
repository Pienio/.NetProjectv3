using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiWizyt.Models;
using DatabaseAccess;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Controllers
{
    public class OpinionController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);

        public ActionResult AddOpinion(long id)
        {
            //ViewBag.ReturnUrl = returnUrl;
            var patient = (Patient)Session["User"];
            var opinion = patient.Opinions.FirstOrDefault(o => o.TargetDoctor.Key == id);
            var doctor = db.Doctors.Find(id);
            if (doctor == null)
                return RedirectToAction("Index", "Home", new { error = "Nie znaleziono lekarza. Spróbuj ponownie." });
            return View(new AddOpinionModel
            {
                DoctorId = id,
                DoctorName = doctor.User.Name.ToString(),
                Creating = opinion == null,
                Rate = opinion?.Rate ?? 0,
                Description = opinion?.Description,
                Nick = opinion?.AuthorNick
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOpinion(AddOpinionModel model)
        {
            var mess = "Dziękujemy za opinię.";
            db.BeginTransaction();
            var doc = db.Doctors.Find(model.DoctorId);
            if (doc == null)
                mess = "Doktor został usunięty z systemu, dlatego nie udało się dodać opinii.";
            else if (model.Creating)
                doc.Opinions.Add(new Opinion
                {
                    AuthorNick = model.Nick,
                    Description = model.Description,
                    TargetDoctor = doc,
                    Author = (Patient)Session["User"],
                    Rate = model.Rate
                });
            else
            {
                var patient = (Patient)Session["User"];
                var opinion = patient.Opinions.First(o => o.Author.Key == patient.Key);
                opinion.Rate = model.Rate;
                opinion.Description = model.Description;
            }
            db.Commit();
            return RedirectToAction("Index", "Home", new { message = mess });
        }

        public ActionResult ShowOpinions(long id)
        {
            var doctor = db.Doctors.Find(id);
            if (doctor == null)
                return RedirectToAction("Index", "Home", new { error = "Nie znaleziono lekarza. Spróbuj ponownie." });
            return View(doctor);
        }
    }
}