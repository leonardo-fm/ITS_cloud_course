using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.BlobStorage;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        public string _user_id { get; set; } = "";
        public ActionResult Home()
        {
            ViewBag.userEmail = Session["userEmail"];
            _user_id = string.IsNullOrEmpty(Session["user_id"] as string) ? _user_id : Session["user_id"] as string;
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

                    string user_id = _user_id; 

                    ConnectionBS cbs = new ConnectionBS(user_id);
                    cbs.userBSManager.addPhotoToUserContainer(ptu);
                });
                t2.Start();

                return Content("Immagine caricata");
            }

            return View();
        }
    }
}