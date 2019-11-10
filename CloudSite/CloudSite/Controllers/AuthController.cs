using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models;
using CloudSite.Models.Log;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.AsyncFunctions;

namespace CloudSite.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserForLogin ufl)
        {
            DBManager dbm = new DBManager();

            User user = dbm.userManager.GetUserData(ufl.userEmailForLogin);
            if (user == null || !user.confirmedEmail)
                return View();

            ConvalidationUser cu = new ConvalidationUser(user);

            if (cu.CheckPasswordIsTheSame(ufl.userPasswordForLogin, user.userPassword))
            {
                Session.Add("user_id", user._id.ToString());
                Session.Add("userEmail", user.userEmail);
                Session.Add("userName", user.userName);

                LogManager.WriteOnLog("user " + user._id.ToString() + " is logged in");

                return RedirectToAction("Home", "Home");
            }

            return View();
        }

        [HttpGet]
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

                if (dbm.userManager.IsTheEmailInTheDB(user.userEmail))
                    return View();

                ConvalidationUser cu = new ConvalidationUser(user);
                if (!cu.isTheUserHaveValidParametres())
                    return View();

                AsyncFunctionToUse.SendMailForConvalidation(user);

                return RedirectToAction("SendedEmail", "Auth");
            }

            return View();
        }

        [HttpGet]
        public ActionResult EmailAuth(string userId)
        {
            DBManager dbm = new DBManager();

            if (userId != null && dbm.userManager.IsTheUserInTheDB(userId))
            {
                dbm.userManager.ConfirmUserToMongoDB(userId);
                return Content("Registrazione avvenuta");
            }
            else
                return Content("Errore");
        }

        [HttpGet]
        public ActionResult SendedEmail()
        {
            return View();
        }
    }
}