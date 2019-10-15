using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Classes
{
    class photo
    {
        public ObjectId _id { get; set; }
        public int userId { get; set; }
        public string photoPath { get; set; }
    }
}
