using System;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;

namespace CloudSite.Model
{
    public class User
    {
        private string _userEmail;
        private string _userPassword;

        public ObjectId _id { get; set; }

        [Required]
        [RegularExpression(@"^(?=[A-Za-z0-9])(?!.*[._()\[\]-]{2})[A-Za-z0-9._()\[\]-]{3,15}$")]
        public string userName{ get; set; }

        [Required]
        public string userEmail
        {
            get { return _userEmail; }
            set
            {
                try
                {
                    MailAddress mail = new MailAddress(value);
                    _userEmail = value;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$")]
        public string userPassword { 
            get { return _userPassword; }
            set 
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
                data = new SHA256Managed().ComputeHash(data);
                string hash = System.Text.Encoding.ASCII.GetString(data);
                _userPassword = hash; ;
            } 
        }

        public bool confirmedEmail { get; set; }
    }
}