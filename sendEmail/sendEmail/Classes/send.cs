using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace sendEmail.Classes
{
    class send
    {
        public static void sendNewEmail(string userEmail, string userName, string bodyText)
        {
            var fromAddress = new MailAddress("ourEmail", "ourName");
            var toAddress = new MailAddress(userEmail, userName);
            const string fromPassword = "ourPassword";
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
