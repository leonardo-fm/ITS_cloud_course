using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CloudSite.Model;
using CloudSite.Models;
using CloudSite.Models.ConvalidationUserAuth;

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

        [HttpPost]
        public ActionResult Login(UserForLogin ufl)
        {
            DBManager dbm = new DBManager();

            User user = dbm.userManager.getUserData(ufl.userEmailForLogin);
            if (user == null || !user.confirmedEmail)
                return View();

            ConvalidationUser cu = new ConvalidationUser(user);

            if (cu.checkPasswordIsTheSame(ufl.userPasswordForLogin, user.userPassword))
                return RedirectToAction("Home", "Home");

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
                //DBManager dbm = new DBManager();

                //if (dbm.userManager.isTheEmailInTheDB(user.userEmail))
                //    return View();

                //ConvalidationUser cu = new ConvalidationUser(user);
                //if (!cu.isTheUserHaveValidParametres())
                //    return View();

                //user.userPassword = cu.cryptUserPassword(user.userPassword);

                //dbm.userManager.addUserToMongoDB(user);

                //DefaultBodyText df = new DefaultBodyText(user.userName, user._id);
                //string text = df.getNewBodyForEmailSubscription();
                //SendMail sm = new SendMail();
                //sm.sendNewEmail(user.userEmail, text);

                return RedirectToAction("SendedEmail", "Auth");
            }

            return View();
        }

        public ActionResult EmailAuth(string userId)
        {
            DBManager dbm = new DBManager();
            if (dbm.userManager.isTheUserInTheDB(userId))
            {
                dbm.userManager.confirmUserToMongoDB(userId);
                return Content("Registrazione avvenuta");
            }
            else
                return Content("Errore");
        }

        public ActionResult SendedEmail()
        {
            return View();
        }
    }
}