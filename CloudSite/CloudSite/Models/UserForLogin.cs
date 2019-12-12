using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSite.Models
{
    public class UserForLogin
    {
        [Required]
        public string UserEmailForLogin { get; set; }

        [Required]
        public string UserPasswordForLogin { get; set; }
    }
}