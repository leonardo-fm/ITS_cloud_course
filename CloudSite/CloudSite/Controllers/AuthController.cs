using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSite.Model;

namespace CloudSite.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(User user)
        {
            if (ModelState.IsValid)
            {
                DBManager dbm = new DBManager();

                if (dbm.userManager.userAlreadyRegistered(user))
                    return View();

                dbm.userManager.addUserToMongoDB(user);

                DefaultBodyText df = new DefaultBodyText(user.userName, user._id);
                string text = df.getNewBodyForEmailSubscription();
                SendMail sm = new SendMail();
                sm.sendNewEmail(user.userEmail, text);

                return RedirectToAction("Index", "Auth");
            }
            return View();
        }

        public ActionResult EmailAuth(string userId)
        {
            DBManager dbm = new DBManager();
            if (dbm.userManager.confirmUserToMongoDB(userId))
                return Content("Registrazione avvenuta");
            else
                return Content("Errore");
        }
    }
}