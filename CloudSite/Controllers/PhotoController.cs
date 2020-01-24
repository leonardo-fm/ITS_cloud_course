using CloudSite.Models.AsyncFunctions;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Photos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudSite.Controllers
{
    public class PhotoController : Controller
    {
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

        [HttpPost]
        public ActionResult DeletePhoto(string photoOS, string photoP)
        {
            List<string> photosToRemove = new List<string> { photoOS, photoP };

            AsyncFunctionToUse.RemoveImages((string)Session["user_id"], photosToRemove);

            return RedirectToAction("Gallery", "Gallery");
        }

        public ActionResult DeletePhotos(Dictionary<string, bool> _PhotosToDelete)
        {
            List<string> photoToDelete = new List<string>();
            foreach (KeyValuePair<string, bool> photo in _PhotosToDelete)
            {
                if (photo.Value == true)
                {
                    photoToDelete.Add(photo.Key);

                    // Add the photoPreview for be delete
                    int indexOfPoint = photo.Key.IndexOf('.');
                    photoToDelete.Add(photo.Key.Insert(indexOfPoint, "_Preview"));
                }
            }

            AsyncFunctionToUse.RemoveImages((string)Session["user_id"], photoToDelete);

            return RedirectToAction("Gallery", "Gallery");
        }
    }
}