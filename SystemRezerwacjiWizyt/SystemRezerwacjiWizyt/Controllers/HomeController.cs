using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiWizyt.Models;
using DatabaseAccess;
using DatabaseAccess.Model;

namespace SystemRezerwacjiWizyt.Controllers
{
    public class HomeController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);

        public ActionResult Index()
        {
            db.Fill();
            var a = db.Doctors.Select(p => p).Where(p => p.ProfileAccepted && p.User.Active).ToList();
            IndexHomeViewModels b= new IndexHomeViewModels();
            b.docs = a;
            b.specs = db.Specializations.Select(p => p).ToList();
            return View(b);
        }

        public ActionResult Search(IndexHomeViewModels model)
        {
            IEnumerable<Doctor> w;
            string namez = model.Text?.ToLower();
            Specialization spec = null;
            if(model.SelSPec!=null&&model.SelSPec!=0)
                spec = db.Specializations.Select(p => p).Where(p => p.Key == model.SelSPec).First();
           
                 

            if (spec == null)
            {
                List<Doctor> a = new List<Doctor>();
                w = db.Doctors.Select(b => b).Where(p => p.ProfileAccepted && p.User.Active);
                if (!string.IsNullOrWhiteSpace(namez))
                {
                    foreach (var VARIABLE in w)
                    {
                        if (VARIABLE.User.Name.ToString().ToLower().Contains(namez))
                            a.Add(VARIABLE);
                    }
                    w = a;
                }
            }
            else
            {
                w =
                    db.Doctors.Select(b => b)
                        .Where(p => p.ProfileAccepted && p.User.Active);
                List<Doctor> a = new List<Doctor>();
                if (!string.IsNullOrWhiteSpace(namez))
                {
                    foreach (var VARIABLE in w)
                    {
                        bool flag = false;
                        foreach (var s in VARIABLE.Specialization)
                        {
                            if (s.Name == spec.Name)
                                flag = true;
                        }

                        if (flag && VARIABLE.User.Name.ToString().ToLower().Contains(namez))
                            a.Add(VARIABLE);
                    }
                }
                else
                {
                    foreach (var VARIABLE in w)
                    {
                        bool flag = false;
                        foreach (var s in VARIABLE.Specialization)
                        {
                            if (s.Name == spec.Name)
                                flag = true;
                        }
                        if (flag)
                            a.Add(VARIABLE);
                    }
                }

                w = a;
            }

            IndexHomeViewModels q = new IndexHomeViewModels();
            q.docs = w.ToList();
            q.specs = db.Specializations.Select(p => p).ToList();
            return View("Index", q);

        }
    }
}