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
        public string userEmailForLogin { get; set; }

        [Required]
        public string userPasswordForLogin { get; set; }
    }
}