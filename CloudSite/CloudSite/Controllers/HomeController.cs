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
        userLoggedIn userLoggedIn = null;

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Home(userLoggedIn uli)
        {
            userLoggedIn = new userLoggedIn(uli);
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                //Task t1 = new Task(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(file));
                //t1.Start();

                Task t2 = new Task(() =>
                {
                    PhotoToUpload ptu = new PhotoToUpload();
                    ptu.photoStream = file;
                    string extension = file.FileName;
                    extension = extension.Substring(extension.IndexOf('.'));
                    ptu.name_id = ObjectId.GenerateNewId() + extension;

                    ConnectionBS cbs = new ConnectionBS(userLoggedIn._id);
                    cbs.userBSManager.addPhotoToUserContainer(ptu);
                });
                t2.Start();

                return Content("Immagine caricata");
            }

            return View();
        }
    }
}