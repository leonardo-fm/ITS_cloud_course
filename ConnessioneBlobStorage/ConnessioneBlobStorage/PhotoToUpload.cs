using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnessioneBlobStorage
{
    class PhotoToUpload
    {
        public ObjectId _id { get; set; }
        public ObjectId _userId { get; set; }
        public byte[] photoStream { get; set; }
    }
}
