using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;

namespace CloudSite.Model
{
    public class User
    {
        private string _email;
        private string _password;

        [Required]
        [RegularExpression(@"^(?=[A-Za-z0-9])(?!.*[._()\[\]-]{2})[A-Za-z0-9._()\[\]-]{3,15}$")]
        public string name{ get; set; }

        [Required]
        public string email
        {
            get { return _email; }
            set
            {
                try
                {
                    MailAddress mail = new MailAddress(value);
                    _email = value;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$")]
        public string password { 
            get { return _password; }
            set 
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
                data = new SHA256Managed().ComputeHash(data);
                string hash = System.Text.Encoding.ASCII.GetString(data);
                _password = hash; ;
            } 
        }
    }
}