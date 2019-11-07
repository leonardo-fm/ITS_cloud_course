using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.AsyncFunctions;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Log;
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
            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            string sasKey = cbs.userBSManager.GetContainerSasUri(cbs.userBSManager.userContainer);

            DBManager dbm = new DBManager();
            List<Photo> photos = dbm.photoManager.getPhotoOfUser((string)Session["user_id"]);
            List<string> imgLinks = new List<string>();
            foreach (Photo photo in photos)
            {
                imgLinks.Add(photo.photoPhat + sasKey);
            }

            ViewBag.images = imgLinks;
            return View();
        }

        [HttpPost]
        public  ActionResult GalleryTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag) && !string.IsNullOrWhiteSpace(tag))
            {
                ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
                string sasKey = cbs.userBSManager.GetContainerSasUri(cbs.userBSManager.userContainer);

                DBManager dbm = new DBManager();
                List<Photo> photos = dbm.photoManager.getPhotoWithTag((string)Session["user_id"], tag);
                List<string> imgLinks = new List<string>();
                foreach (Photo photo in photos)
                {
                    imgLinks.Add(photo.photoPhat + sasKey);
                }

                ViewBag.images = imgLinks;
                return View();
            }

            return RedirectToAction("Gallery", "Home");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType.StartsWith("image/"))
            {
                LogManager.writeOnLog("user " + (string)Session["user_id"] + " uploaded an image with name " + file.FileName);

                AsyncFunctionToUse.uploadPhoto(file, (string)Session["user_id"]);

                return View();
            }

            return RedirectToAction("Home", "Home");
        }
    }
}