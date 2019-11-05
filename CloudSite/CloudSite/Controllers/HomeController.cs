using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.MoongoDB;
using CloudSite.Models;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using Azure.Identity;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Gallery()
        {
            //DBManager dbm = new DBManager();
            //List<Photo> photos = dbm.photoManager.getPhotoOfUser((string)Session["user_id"]);
            //List<string> name = new List<string>();
            //foreach (Photo photo in photos)
            //{
            //    name.Add(photo._id.ToString());
            //}

            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            Task<string> t1 = new Task<string>(() => cbs.userBSManager.getUserSasKey("progettocloudstorage", (string)Session["user_id"]));
            t1.Start();
            t1.Wait();

            return Content(t1.Result);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType.StartsWith("image/"))
            {
                string extension = file.FileName.Substring(file.FileName.IndexOf('.'));

                Photo userPhoto = new Photo();

                Stream photoForCV = new MemoryStream();
                Stream photoForBS = new MemoryStream();

                using (Stream photo = file.InputStream)
                {
                    photo.CopyTo(photoForCV);

                    photoForCV.Seek(0, SeekOrigin.Begin);
                    photo.Seek(0, SeekOrigin.Begin);

                    photo.CopyTo(photoForBS);
                    photoForBS.Seek(0, SeekOrigin.Begin);
                    photo.Seek(0, SeekOrigin.Begin);
                }

                Task<string[]> t1 = new Task<string[]>(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(photoForCV));
                Task t2 = new Task(() => uploadPhotoToBlobStorage((string)Session["user_id"], photoForBS, extension, ref userPhoto));

                t1.Start();
                t2.Start();

                Task.WhenAll(t1, t2).Wait();

                userPhoto.tags = t1.Result;
                userPhoto._userId = (string)Session["user_id"];

                DBManager dbm = new DBManager();
                dbm.photoManager.addPhotoToMongoDB(userPhoto);

                return View();
            }

            return RedirectToAction("Home", "Home");
        }

        private void uploadPhotoToBlobStorage(string userId, Stream photo, string extension, ref Photo userPhoto)
        {
            userPhoto._id = ObjectId.GenerateNewId();
            string photoName = userPhoto._id + extension;
            userPhoto.photoPhat = string.Format("https://progettocloudstorage.blob.core.windows.net/{0}/{1}", userId, photoName);

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.userBSManager.addPhotoToUserContainer(photo, photoName);
        }
    }
}