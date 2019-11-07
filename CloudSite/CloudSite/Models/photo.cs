using System;
using MongoDB.Bson;
using System.Collections.Generic;

namespace CloudSite.Models
{
    public class Photo
    {
        public ObjectId _id { get; set; }
        public string imageName { get; set; }
        public string _userId { get; set; }
        public string[] tags { get; set; }
        public string photoPhat { get; set; }

        /*EXIF*/
        public string photoGpsLatitude { get; set; }    //0x0002
        public string photoGpsLongitude { get; set; }   //0x0004
        public string photoTagDateTime { get; set; }    //0x0132
        public string photoTagImageWidth { get; set; }  //0x0100
        public string photoTagImageHeight { get; set; } //0x0101

    }
}
