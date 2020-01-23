using System;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.AsyncFunctions;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Log;
using CloudSite.Models.Photos;
using System.Collections.Generic;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Home()
        {
            if(Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        public ActionResult Gallery()
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            string sasKey = cbs.UserBSManager.GetContainerSasUri();

            DBManager dbm = new DBManager();
            List<Photo> photos = dbm.PhotoManager.GetPhotosOfUser((string)Session["user_id"]);
            foreach (Photo photo in photos)
            {
                photo.PhotoPhatWhitSasKey = photo.PhotoPhatPreview + sasKey;
            }
            return View(photos);
        }

        public ActionResult UploadPhoto(string msg = "")
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            return View("UploadPhoto", null, msg);
        }

        [HttpPost]
        public  ActionResult GalleryTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag) && !string.IsNullOrWhiteSpace(tag))
            {
                tag = tag.ToLower();

                ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
                string sasKey = cbs.UserBSManager.GetContainerSasUri();

                DBManager dbm = new DBManager();
                List<Photo> photos = dbm.PhotoManager.GetPhotosWithTag((string)Session["user_id"], tag);
                foreach (Photo photo in photos)
                {
                    photo.PhotoPhatWhitSasKey = photo.PhotoPhatPreview + sasKey;
                }

                return View("Gallery", photos);
            }

            return RedirectToAction("Gallery", "Home");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType.Contains("image"))
            {
                LogManager.WriteOnLog("user " + (string)Session["user_id"] + " uploaded an image with name " + file.FileName);

                AsyncFunctionToUse.UploadPhoto(file, (string)Session["user_id"]);

                return RedirectToAction("UploadPhoto", "Home", new { msg = "Uploaded with success" });
            }

            return RedirectToAction("UploadPhoto", "Home", new { msg = "Wrong file content type" });
        }

        public ActionResult SinglePhoto(string photoId)
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            DBManager dbm = new DBManager();
            PhotoShare p = new PhotoShare();
            p.PhotoToShare = dbm.PhotoManager.GetPhotoForDetails((string)Session["user_id"], photoId);

            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            p.PhotoToShare.PhotoPhatWhitSasKey = p.PhotoToShare.PhotoPhatOriginalSize + cbs.UserBSManager.GetContainerSasUri();

            return View(p);
        }

        [HttpPost]
        public ActionResult SharePhoto(string photoPath, PhotoShare photoShare)
        {
            DateTime finalDate = new DateTime(
                photoShare.DateOfExpire.Year, 
                photoShare.DateOfExpire.Month, 
                photoShare.DateOfExpire.Day, 
                photoShare.TimeOfExpire.Hours, 
                photoShare.TimeOfExpire.Minutes, 
                photoShare.TimeOfExpire.Seconds);

            int totalMinutes = (int)finalDate.Subtract(DateTime.Now).TotalMinutes;
            totalMinutes = totalMinutes <= 0 ? 0 : totalMinutes;

            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            string sasKey = cbs.UserBSManager.GetContainerSasUri(totalMinutes);

            return Content(photoPath + sasKey);
        }

        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Auth");
        }
    }
}