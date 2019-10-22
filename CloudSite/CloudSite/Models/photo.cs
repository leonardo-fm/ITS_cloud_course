using System;
using MongoDB.Bson;
using System.Collections.Generic;

namespace CloudSite.Model
{
    public class Photo
    {
        public ObjectId _id { get; set; }
        public ObjectId _userId { get; set; }
        public List<string> tags { get; set; }
        public string photoPath { get; set; }
    }
}
