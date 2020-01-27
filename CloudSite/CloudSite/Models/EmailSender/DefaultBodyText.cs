using MongoDB.Bson;
using System.Web;

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
            msg += Variables.URL_OF_THE_HOST + @"Auth/EmailAuth?userId=" + _id + "\n\n";
            msg += "In caso non sia stato lei a sottoscriversi, ignori questa mail.\n\n";
            msg += "Buna giornata da Lo Fra";

            return msg;
        }
    }
}
