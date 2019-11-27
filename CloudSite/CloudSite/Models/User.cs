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
        public string userName{ get; set; }

        [Required]
        public string userEmail { get; set; }

        [Required]
        public string userPassword { get; set; }

        public bool confirmedEmail { get; set; }
    }
}