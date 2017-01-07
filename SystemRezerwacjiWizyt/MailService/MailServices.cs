using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Model;

namespace MailService
{
    public class MailServices
    {
        public MailServices()
        {
        }

        public void SendSimpleMail()
        {
            SendMail("pienbartl@gmail.com", "test", "Test", "subtest", "testest");
        }

        public void SendRegistrationConfirmation(string mailAddress, string token)
        {
            string content = "Witamy w Systemie rezerwacji wizyt.\n" +
                "Aby potwierdzić rejestrację, wpisz w wymaganym polu dany token: " + token;
            SendMail(mailAddress, "Witamy w Systemie rezerwacji wizyt", "System rezerwacji wizyt", "Potwierdzenie rejestracji", content);
        }
        public void SendEditConfirmation(string mailAddress, string token)
        {
            string content =
                "Aby potwierdzić edycję, wpisz w wymaganym polu dany token: " + token;
            SendMail(mailAddress, "Edycja konta", "System rezerwacji wizyt", "Potwierdzenie edycji konta", content);
        }

        public void SendMailChangeConfirmation(string mailAddress, string token)
        {
            string content = "Aby potwierdzić zmianę adresu e-mail, wpisz w wymaganym polu dany token: " + token;
            SendMail(mailAddress, "Zmiana adresu e-mail", "System rezerwacji wizyt", "Potwierdzenie zmiany adresu e-mail", content);
        }

        public void SendAcceptationMail(string mailAddress)
        {
            string content = "Zostałeś zaakceptowany w Systemie rezerwacji wizyt. Możesz już zalogować się na swoje konto.";
            SendMail(mailAddress, "Akceptacja rezerwacji", "System rezerwacji wizyt", "Akceptacja rezerwacji", content);
        }

        public void SendAcceptationEditMail(string mailAddress)
        {
            string content = "Twoja prośba o edycję konta w Systemie rezerwacji wizyt została zaakceptowana.";
            SendMail(mailAddress, "Akceptacja edycji konta", "System rezerwacji wizyt", "Akceptacja rezerwacji", content);
        }

        public void SendVisitRegistrationNotifications(Doctor doc, Patient pac, DateTime date)
        {
            string content = "Potwierdzono rejestrację na wizytę u lekarza " + doc.User.Name + " w terminie " + date.ToString(CultureInfo.CurrentCulture) + ".";
            SendMail(pac.User.Mail, "Rezerwacja wizyty", "System rezerwacji wizyt", "Potwierdzenie rezerwacji", content);

            content = "Pacjent "+ pac.User.Name+" zarejestrował się na wizytę u Ciebie w terminie " + date.ToString(CultureInfo.CurrentCulture) + ".";
            SendMail(doc.User.Mail, "Rezerwacja wizyty", "System rezerwacji wizyt", "Powiadomienie o rezerwacji", content);
        }
        public void SendVisitDeleteNotification(Visit visit, DocOrPat currentUserType)
        {
            string contentPac, contentDoc;
            switch (currentUserType)
            {
                case DocOrPat.Doctor:
                    contentDoc = "Odwołano wizytę z pacjentem " + visit.Patient.User.Name + " w terminie " + visit.Date.ToString(CultureInfo.CurrentCulture) + ".";
                    contentPac = "Lekarz " + visit.Doctor.User.Name + " odwołał wizytę w terminie " + visit.Date.ToString(CultureInfo.CurrentCulture) + ".";
                    break;
                case DocOrPat.Patient:
                    contentDoc = "Pacjent " + visit.Patient.User.Name + " odwołał wizytę w terminie " + visit.Date.ToString(CultureInfo.CurrentCulture) + ".";
                    contentPac = "Odwołano wizytę u lekarza " + visit.Doctor.User.Name + " w terminie " + visit.Date.ToString(CultureInfo.CurrentCulture) + ".";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentUserType), currentUserType, null);
            }
            SendMail(visit.Doctor.User.Mail, "Odwołanie wizyty", "System rezerwacji wizyt", "Powiadomienie o odwołaniu wizyty", contentDoc);
            SendMail(visit.Patient.User.Mail, "Odwołanie wizyty", "System rezerwacji wizyt", "Powiadomienie o odwołaniu wizyty", contentPac);
        }

        public void SendRejectionMail(string mailAddress, string reason)
        {
            string content = "Niestety, twoja prośba o rejestrację została rozpatrzona negatywnie.\nPowód: " + reason;
            SendMail(mailAddress, "Odrzucenie rejestracji", "System rezerwacji wizyt", "Odrzucenie rejestracji", content);
        }

        public void SendEditRejectionMail(string mailAddress, string reason)
        {
            string content = "Niestety, twoja prośba o edycję została rozpatrzona negatywnie.\nPowód: " + reason;
            SendMail(mailAddress, "Odrzucenie rejestracji", "System rezerwacji wizyt", "Odrzucenie rejestracji", content);
        }

        private void SendMail(string address, string title, string header, string subheader, string description)
        {
            using (var client = CreateClient())
            {
                ResourceManager mng = new ResourceManager(typeof(Properties.Resources));
                string Body = mng.GetString("index");
                Body = Body.Replace("#Header#", header);
                Body = Body.Replace("#SubHeader#", subheader);
                Body = Body.Replace("#Opis#", description);

                using (MailMessage mm = new MailMessage("sysrejwiz@gmail.com", address, title, Body))
                {
                    mm.IsBodyHtml = true;
                    mm.BodyEncoding = UTF8Encoding.UTF8;
                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mm);
                }
            }
        }
            
        private SmtpClient CreateClient()
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("sysrejwiz@gmail.com", "qwerasdfzxcv");
            return client;
        }
    }
}
