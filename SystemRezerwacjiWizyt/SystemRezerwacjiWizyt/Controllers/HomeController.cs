using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;

namespace SystemRezerwacjiWizyt.Controllers
{
    public class HomeController : Controller
    {
        private ITransactionalApplicationData db = new ApplicationDataFactory().CreateTransactionalApplicationData(false);

        public ActionResult Index()
        {   
            db.Fill();
            var a = db.Doctors.Select(p => p).Where(p => p.ProfileAccepted && p.User.Active).ToList();
            return View(a);
        }

       
    }
}