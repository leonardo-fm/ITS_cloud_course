using CloudSite.Models.BlobStorage;
using CloudSite.Models.ModelForViews;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Photos;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CloudSite.Controllers
{
    public class GalleryController : Controller
    {
        public ActionResult Gallery()
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            List<Photo> photos = GetPhotosForTheGallery();

            return View(photos);
        }

        public ActionResult GalleryTag(string tag)
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            if (!string.IsNullOrEmpty(tag) && !string.IsNullOrWhiteSpace(tag))
            {
                tag = tag.ToLower();

                List<Photo> photos = GetPhotosForTheGallery(tag);

                return View("Gallery", photos);
            }

            return RedirectToAction("Gallery", "Gallery");
        }

        public ActionResult GalleryDelete()
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            List<Photo> photos = GetPhotosForTheGallery();

            Dictionary<string, bool> photosToDelete = new Dictionary<string, bool>();

            foreach (Photo photo in photos)
            {
                photosToDelete.Add(photo.PhotoNameOriginalSize, false);
            }

            ModelForGalleryDelete mfgd = new ModelForGalleryDelete(photos, photosToDelete);
            return View(mfgd);
        }

        private List<Photo> GetPhotosForTheGallery(string tag = "")
        {
            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            string sasKey = cbs.UserBSManager.GetContainerSasUri();

            List<Photo> photos = new List<Photo>();

            DBManager dbm = new DBManager();

            if (tag == "")
                photos = dbm.PhotoManager.GetPhotosOfUser((string)Session["user_id"]);
            else
                photos = dbm.PhotoManager.GetPhotosWithTag((string)Session["user_id"], tag);


            foreach (Photo photo in photos)
                photo.PhotoPhatWhitSasKey = photo.PhotoPhatPreview + sasKey;

            return photos;
        }
    }
}