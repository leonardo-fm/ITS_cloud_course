using System;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace CloudSite.Models
{
    public class User
    {
        public ObjectId _id { get; set; }

        [Required]
        public string UserName{ get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string UserPassword { get; set; }

        public bool ConfirmedEmail { get; set; }
    }
}