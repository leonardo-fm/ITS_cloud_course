using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.BlobStorage;
using CloudSite.Models;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                //Task t1 = new Task(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(file));
                //t1.Start();
               
                Task t2 = new Task(() => uploadPhotoToBlobStorage((string)Session["user_id"], file));
                t2.Start();

                t2.Wait();

                return Content("Immagine caricata");
            }

            return View();
        }

        private void uploadPhotoToBlobStorage(string userId, HttpPostedFileBase file)
        {
            PhotoToUpload ptu = new PhotoToUpload();
            ptu.photoStream = file;
            string extension = file.FileName;
            extension = extension.Substring(extension.IndexOf('.'));
            ptu.name_id = ObjectId.GenerateNewId() + extension;

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.userBSManager.addPhotoToUserContainer(ptu);
        }
    }
}