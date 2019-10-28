using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CloudSite.Model;
using CloudSite.Models;
using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.ComputerVision;

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
            {
                Session["user_id"] = user._id;
                Session["userName"] = user.userName;
                Session["userEmail"] = user.userEmail;
    
                return RedirectToAction("Home", "Home");
            }

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
                //dbmanager dbm = new dbmanager();

                //if (dbm.usermanager.istheemailinthedb(user.useremail))
                //    return view();

                //convalidationuser cu = new convalidationuser(user);
                //if (!cu.istheuserhavevalidparametres())
                //    return view();

                //user.userpassword = cu.cryptuserpassword(user.userpassword);

                //dbm.usermanager.addusertomongodb(user);

                //defaultbodytext df = new defaultbodytext(user.username, user._id);
                //string text = df.getnewbodyforemailsubscription();
                //sendmail sm = new sendmail();
                //sm.sendnewemail(user.useremail, text);

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