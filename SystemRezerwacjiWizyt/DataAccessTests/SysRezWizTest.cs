using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SystemRezerwacjiWizyt.Models;
using DatabaseAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseAccess.Model;

namespace UnitTestProject1
{
    [TestClass]
    public class DataServiceTests
    {
        IApplicationDataFactory factory= new ApplicationDataFactory();
        private ITransactionalApplicationData db;

        [TestInitialize]
        public void Initialize()
        {

           db= factory.CreateTransactionalApplicationData(false);


            // Dtb = factory.CreateApplicationData();
        }

        [TestMethod]
        public void GetDoctor()
        {
            
            Random r = new Random();
            int d = r.Next(1, 10);
            var doc = db.Doctors.Find(d);

            if (d > 8)
            {
                if (doc != null)
                    Assert.Fail();

            }
            else
            {
                if (doc == null)
                    Assert.Fail();
            }

          

        }
        [TestMethod]
        public void GetUser()
        {

            string[] pesels = { "09586749381", "19683750923", "94860285691", "58672349682", "38596827364", "58476923857", "88975643287", "29384795618" };
            string password = "96e79218965eb72c92a549dd5a330112";

           
            Random r = new Random();
            int d = r.Next(1, 10);

            if (d <= 7)
            {
                var doc = db.Users.Select(p => p).Where(p => p.PESEL == pesels[d] && p.Password == password);
                if (doc == null)
                    Assert.Fail();

            }
         



        }
        
        [TestMethod]
        public void GetDoctorsList()
        {


            var doc = db.Doctors.Select(p=>p);

            if (doc.Count() == 0)
                Assert.Fail();

        }
        [TestMethod]
        public void SearchDoctors()
        {

            Specialization[] specs = {
            new Specialization("Reumatolog"),
            new Specialization("Kardiolog"),
            new Specialization("Neurolog"),
            new Specialization("Urolog"),
            new Specialization("Okulista"),
            new Specialization("Psychiatra"),
            new Specialization("Ginekolog"),
            new Specialization("Pediatra")};
            string[] names = { "Kuba", "Jan", "Łukasz", "Adrian", "Bartosz", "Marek", "Filip", "Bartłomiej" };
            string[] surnames = { "Soczkowski", "Berwid", "Okular", "Michałowski", "Skała", "Mikowski", "Wasiłkowski", "Normowski" };
         
            Random r = new Random();
            int d = r.Next(0, 7);

            var c = new SystemRezerwacjiWizyt.Controllers.HomeController();
            IndexHomeViewModels nw = new IndexHomeViewModels();
            
            nw.Text = names[d] + " " + surnames[d];
            nw.SelSPec = d+1;
            var g = c.Search(nw);
            ViewResult viewResult = (ViewResult)g;
            nw =(IndexHomeViewModels)viewResult.Model;

            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }

            nw.Text = names[d] + " " + surnames[d];
            nw.SelSPec = null;
            g = c.Search(nw);
            viewResult = (ViewResult)g;
            nw = (IndexHomeViewModels)viewResult.Model;
            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }
            nw.Text =surnames[d];
            nw.SelSPec = d+1;
            g = c.Search(nw);
            viewResult = (ViewResult)g;
            nw = (IndexHomeViewModels)viewResult.Model;
            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }
            nw.Text = names[d];
            nw.SelSPec = d+1;
            g = c.Search(nw);
            viewResult = (ViewResult)g;
            nw = (IndexHomeViewModels)viewResult.Model;
            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }
          


        }
        [TestMethod]
        public void GetSpecList()
        {


            var a = new SystemRezerwacjiWizyt.Controllers.HomeController();
            IndexHomeViewModels s = new IndexHomeViewModels();
            var g = a.Index("none","none");
            ViewResult viewResult = (ViewResult)g;
            s = (IndexHomeViewModels)viewResult.Model;
         

            if (s.docs == null||s.docs.Count==0)
                Assert.Fail();

        }
        [TestMethod]
        public void GetOpinionList()
        {


            var a = new SystemRezerwacjiWizyt.Controllers.OpinionController();
            Doctor s = new Doctor();
            Random r = new Random();
            int d = r.Next(1, 8);
            var g = a.ShowOpinions(d);
            ViewResult viewResult = (ViewResult)g;
            s = (Doctor)viewResult.Model;


            if (s.Opinions.Count != 0)
                Assert.Fail();

        }
        [TestMethod]
        public void CheckAddandDeleteDoctor()
        {


         
            var doc = TestingExtension.GetDoctor();
            var cer = db.Specializations.Select(p=>p).ToArray();
            var spec = cer[0];
            doc.Specialization = new List<Specialization>();
            doc.Specialization.Add(spec);
            db.BeginTransaction();
            db.Doctors.Add(doc);
            db.Commit();
            var a = db.Doctors.Select(p=>p).Where(p=>p.User.PESEL==doc.User.PESEL);
            if(a==null||a.Count()==0)
                Assert.Fail();

           
            db.BeginTransaction();
            var doccc = a.First();
            long f = doccc.Key;
            db.Users.Remove(doccc.User);
            db.Doctors.Remove(doccc);
            db.Commit();
            var aa = db.Doctors.Find(f);
            if(aa!=null)
                Assert.Fail();
           



        }
        [TestMethod]
        public void CheckAddandDeletePatient()
        {


            

            var doc = TestingExtension.GetPatient();
            db.BeginTransaction();
            db.Patients.Add(doc);
            db.Commit();
            string psl = doc.User.PESEL;
            var c = db.Patients.Select(p=>p).Where(p=>p.User.PESEL==psl);
            if (c == null || c.Count() == 0)
                Assert.Fail();


            db.BeginTransaction();
            long f = c.First().Key;
            db.Users.Remove(c.First().User);
            db.Patients.Remove(c.First());
            db.Commit();
            var cc = db.Patients.Find(f);
            if (cc != null)
                Assert.Fail();




        }



        [TestMethod]
        public void CheckAddandDeleteVisit()
        {


         

            var pac = TestingExtension.GetPatient();
            var doc = TestingExtension.GetDoctor();
            doc.Specialization= new List<Specialization>();
            doc.Specialization.Add(db.Specializations.Find(1));
            db.BeginTransaction();
            db.Patients.Add(pac);
            db.Doctors.Add(doc);
            db.Commit();
            var c = db.Doctors.Select(p => p).Where(p => p.User.PESEL == doc.User.PESEL).First();
            var d = db.Patients.Select(p => p).Where(p => p.User.PESEL == pac.User.PESEL).First();
            Visit dd = new Visit();
            dd.Patient = d;
            dd.Doctor = c;
            dd.Spec = c.Specialization.First();
            dd.Date = DateTime.Now;
            d.Visits.Add(dd);
            c.Visits.Add(dd);
            db.BeginTransaction();
            db.Visits.Add(dd);
            db.Commit();
            var k = db.Visits.Select(p => p).Where(p => p.Doctor.Key == c.Key);
            if(k==null||k.Count()==0)
                Assert.Fail();
            var vd = k.First();
            db.BeginTransaction();
            var patt = db.Patients.Find(vd.Patient.Key);
            var docc = db.Doctors.Find(vd.Doctor.Key);
            patt.Visits.Remove(vd);
            docc.Visits.Remove(vd);
            db.Visits.Remove(vd);
            db.Commit();
             k = db.Visits.Select(p => p).Where(p => p.Doctor.Key == c.Key);
            if (k.Count() != 0)
                Assert.Fail();

            db.BeginTransaction();
            db.Users.Remove(patt.User);
            db.Users.Remove(docc.User);
            db.Patients.Remove(patt);
            db.Doctors.Remove(docc);
            db.Commit();





        }


    }
}
