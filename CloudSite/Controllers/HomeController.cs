using System;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.AsyncFunctions;
using CloudSite.Models.Log;

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
                LogManager.WriteOnLog("user " + (string)Session["user_id"] + " uploaded an image with name " + file.FileName);

                AsyncFunctionToUse.UploadPhoto(file, (string)Session["user_id"]);

                return RedirectToAction("UploadPhoto", "Home", new { msg = "Uploaded with success" });
            }

            return RedirectToAction("UploadPhoto", "Home", new { msg = "Wrong file content type" });
        }

        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Auth");
        }
    }
}