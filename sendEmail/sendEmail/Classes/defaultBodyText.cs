using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sendEmail.Classes
{
    class defaultBodyText
    {
        public static string getNewBodyForEmailSubscription(string userName, string convalidationLinkForSubscription)
        {
            string msg = "";

            msg += "Benvenuto " + userName + "!\n\n";
            msg += "Per confermare la tua email premere sul link seguente:\n";
            msg += convalidationLinkForSubscription + "\n\n";
            msg += "In caso non sia stato lei a sottoscriversi, ignori questa mail.\n\n";
            msg += "Buna giornata da Lo Fra";

            return msg;
        }
    }
}
