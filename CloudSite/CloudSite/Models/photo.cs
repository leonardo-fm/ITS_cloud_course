using System;
using MongoDB.Bson;
using System.Collections.Generic;

namespace CloudSite.Models
{
    public class Photo
    {
        public ObjectId _id { get; set; }
        public string _userId { get; set; }
        public string[] tags { get; set; }
        public string photoPhat { get; set; }
    }
}
