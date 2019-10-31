using MongoDB.Bson;
using System.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSite.Models.BlobStorage
{
    class PhotoToUpload
    {
        public string name_id { get; set; }
        public HttpPostedFileBase photoStream { get; set; }
    }
}
