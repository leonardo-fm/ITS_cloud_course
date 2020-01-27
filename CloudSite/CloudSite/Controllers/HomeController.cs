using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.AsyncFunctions;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.LogManager;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Photos;
using CloudSite.Models.User;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        public ActionResult UploadPhoto(string msg = "")
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            return View("UploadPhoto", null, msg);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType.Contains("image"))
            {
                if (file.ContentLength > 5000000)
                    return RedirectToAction("UploadPhoto", "Home", new { msg = "The file is too big, max 5 MB" });

                // Log
                LogMaster.WriteOnLog("user " + (string)Session["user_id"] + " uploaded an image with name " + file.FileName);

                AsyncFunctionToUse.UploadPhoto(file, (string)Session["user_id"]);

                return RedirectToAction("UploadPhoto", "Home", new { msg = "Uploaded with success" });
            }

            return RedirectToAction("UploadPhoto", "Home", new { msg = "Wrong file content type" });
        }

        public ActionResult Options()
        {
            if (Session["login"] == null)
                return RedirectToAction("Login", "Auth");

            DBManager dbm = new DBManager();
            UserModel user = dbm.UserManager.GetUserData((string)Session["userEmail"]);

            return View(user);
        }

        [HttpPost]
        public ActionResult DeleteAccount(string confirmUsername)
        {
            if(confirmUsername != (string)Session["userName"])
                return RedirectToAction("Options", "Home");

            // Log
            LogMaster.WriteOnLog("user " + (string)Session["user_id"] + " delete his/her account");

            ClearUserData();

            return RedirectToAction("Logout", "Home");
        }

        private void ClearUserData()
        {
            DBManager dbm = new DBManager();
            List<Photo> allUserPhotos = dbm.PhotoManager.GetPhotosOfUser((string)Session["user_id"]);

            List<string> photoToDelete = new List<string>();

            foreach (Photo photo in allUserPhotos)
            {
                photoToDelete.Add(photo.PhotoNameOriginalSize);
                photoToDelete.Add(photo.PhotoNamePreview);
            }

            AsyncFunctionToUse.RemoveImages((string)Session["user_id"], photoToDelete);

            ConnectionBS cbs = new ConnectionBS((string)Session["user_id"]);
            cbs.UserBSManager.DeleteUserContainer();

            dbm.UserManager.DeleteUserFromDB((string)Session["user_id"]);
        }

        public ActionResult Logout()
        {
            // Log
            LogMaster.WriteOnLog("user " + (string)Session["user_id"] + " logout");

            Session.Abandon();

            return RedirectToAction("Index", "Auth");
        }
    }
}