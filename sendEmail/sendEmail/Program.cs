using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sendEmail.Classes;

namespace sendEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            string userEmail = "example@example.com";
            string userName = "userName";
            string linkConvalidation = @"linkForConvalidation";
            string bodyText = defaultBodyText.getNewBodyForEmailSubscription(userName, linkConvalidation);

            send.sendNewEmail(userEmail, bodyText);

            Console.WriteLine("Email sended successfully");
            Console.ReadKey();
        }
    }
}
