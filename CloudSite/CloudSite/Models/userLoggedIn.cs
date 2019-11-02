
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSite.Models
{
    public class userLoggedIn
    {
        [Required]
        public string _id { get; set; }

        [Required]
        public string userName { get; set; }

        [Required]
        public string userEmail { get; set; }

        public userLoggedIn(string _id, string userName, string userEmail)
        {
            this._id = _id;
            this.userName = userName;
            this.userEmail = userEmail;
        }

        public userLoggedIn(userLoggedIn uli)
        {
            _id = uli._id;
            userName = uli.userName;
            userEmail = uli.userEmail;
        }

    }
}