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
            nw.SelSPec = d;
            var g = c.Search(nw);
            ViewResult viewResult = (ViewResult)g;
            nw =(IndexHomeViewModels)viewResult.Model;

            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }

            nw.Text = names[d] + " " + surnames[d];
            g = c.Search(nw);
            viewResult = (ViewResult)g;
            nw = (IndexHomeViewModels)viewResult.Model;
            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }
            nw.Text =surnames[d];
            nw.SelSPec = d;
            g = c.Search(nw);
            viewResult = (ViewResult)g;
            nw = (IndexHomeViewModels)viewResult.Model;
            if (nw.docs == null || nw.docs.Count == 0)
            {
                Assert.Fail();
            }
            nw.Text = names[d];
            nw.SelSPec = d;
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
        public void CheckAddandDeleteDoctor()
        {


         
            var doc = TestingExtension.GetDoctor();
            var cer = db.Specializations.Select(p=>p).ToArray();
            var spec = cer[0];
            doc.Specialization = new List<Specialization>();
            doc.Specialization.Add(spec);
            db.Doctors.Add(doc);
            var a = db.Doctors.Find();
            //SystemRezerwacjiWizyt.Models.RegisterDoctorViewModel newdoc = new RegisterDoctorViewModel();
            //newdoc.doc = doc;
            //newdoc.SpecId = 1;
            //newdoc.PESEL = doc.User.PESEL;
            //newdoc.Mail = doc.User.Mail;
            //newdoc.Password = "111111";
            //newdoc.PasswordAgain = "111111";
            //newdoc.Name = new PersonName();
            //newdoc.Name.Name = "asd";
            //newdoc.Name.Surname = "dsa";
            //var adding = SystemRezerwacjiWizyt.Controllers.AccountController
            //var k = a.AddDoctor(doc);

            //Assert.IsTrue(k);

            //var c = a.SearchDoctorsList(null, "Janowski");


            //if (c == null || c.Length == 0)
            //    Assert.Fail();

            //var dd = a.DeleteDoctor(c.First());
            //Assert.IsTrue(dd);




        }
        //[TestMethod]
        //public void CheckAddandDeletePatient()
        //{


        //    var a = TestingExtension.GetService();

        //    var doc = TestingExtension.GetPatient();

        //    a.AddPatient(doc);
        //    var c = a.GetUser(doc.User.PESEL, doc.User.Password);
        //    var d = a.GetPatientByUserId((int)c.Key);
        //    if (d == null)
        //        Assert.Fail();

        //    var dd = a.DeletePatient(d);
        //    Assert.IsTrue(dd);




        //}

        //[TestMethod]
        //public void CheckAddandDeleteVisit()
        //{


        //    var a = TestingExtension.GetService();

        //    var pac = TestingExtension.GetPatient();
        //    var doc = TestingExtension.GetDoctor();

        //    a.AddPatient(pac);
        //    a.AddDoctor(doc);
        //    var c = a.GetUser(pac.User.PESEL, pac.User.Password);
        //    var pac1 = a.GetPatientByUserId((int)c.Key);
        //    var doc1 = a.SearchDoctorsList(null, "Janowski");
        //    if (doc1.Length == 0)
        //        Assert.Fail();
        //    var doc2 = doc1.First();
        //    var date = a.GetFirstFreeSlot((int)doc2.Key);

        //    bool dd = a.RegisterVisit(date, (int)pac1.Key, (int)doc2.Key);
        //    Assert.IsTrue(dd);
        //    var pacviz = a.GetPatientVisits((int)pac1.Key, false);
        //    var docviz = a.GetDoctorVisits((int)doc2.Key, false);
        //    if (pacviz.Length == 0 || docviz.Length == 0)
        //        Assert.Fail();

        //    dd = a.DeleteVisit(pacviz.First());
        //    Assert.IsTrue(dd);

        //    dd = a.DeletePatient(pac1);
        //    Assert.IsTrue(dd);
        //    dd = a.DeleteDoctor(doc2);
        //    Assert.IsTrue(dd);


        //}


    }
}
