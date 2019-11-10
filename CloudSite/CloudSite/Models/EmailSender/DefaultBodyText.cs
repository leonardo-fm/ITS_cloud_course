using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CloudSite.Models.EmailSender
{
    class DefaultBodyText
    {
        private string _userName;
        private string _id;

        public DefaultBodyText(string userName, ObjectId id)
        {
            _userName = userName;
            _id = id.ToString();
        }

        public string GetNewBodyForEmailSubscription()
        {
            string msg = "";

            msg += "Benvenuto " + _userName + "!\n\n";
            msg += "Per confermare la tua email premere sul link seguente:\n";
            msg += @"https://localhost:44385/Auth/EmailAuth/" + _id + "\n\n";
            msg += "In caso non sia stato lei a sottoscriversi, ignori questa mail.\n\n";
            msg += "Buna giornata da Lo Fra";

            return msg;
        }
    }
}
