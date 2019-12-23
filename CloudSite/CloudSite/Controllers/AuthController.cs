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

            User user = dbm.UserManager.GetUserData(ufl.UserEmailForLogin);
            if (user == null || !user.ConfirmedEmail)
                return View();

            ConvalidationUser cu = new ConvalidationUser(user);

            if (cu.CheckPasswordIsTheSame(ufl.UserPasswordForLogin, user.UserPassword))
            {
                Session["user_id"] = user._id.ToString();
                Session["userEmail"] = user.UserEmail;
                Session["userName"] = user.UserName;
                Session["login"] = true;

                LogManager.WriteOnLog("user " + (string)Session["user_id"] + " is logged in");

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

                if (dbm.UserManager.IsTheEmailInTheDB(user.UserEmail))
                    return View();

                ConvalidationUser cu = new ConvalidationUser(user);
                if (!cu.isTheUserHaveValidParametres())
                    return View();

                AsyncFunctionToUse.SendMailForConvalidation(user);

                return Content("Abbiamo inviato un'email di conferma");
            }

            return View();
        }

        [HttpGet]
        public ActionResult EmailAuth(string userId)
        {
            DBManager dbm = new DBManager();

            if (userId != null && dbm.UserManager.IsTheUserInTheDB(userId))
            {
                dbm.UserManager.ConfirmUserToMongoDB(userId);
                return RedirectToAction("Login");
            }
            
            return Content("Errore");
        }
    }
}