using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using CloudSite.Models;

namespace CloudSite.Model
{
    class SendMail
    {
        public SendMail() { }

        public void sendNewEmail(string userEmail, string bodyText)
        {
            var fromAddress = new MailAddress(Variables.EMAIL_ADDRESS_FOR_SENDING_EMAILS, "Lo Fra");
            var toAddress = new MailAddress(userEmail);
            const string fromPassword = Variables.PASSWORD_FOR_EMAIL_ADDRESS;
            string subject = "Subscription";
            string body = bodyText;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
