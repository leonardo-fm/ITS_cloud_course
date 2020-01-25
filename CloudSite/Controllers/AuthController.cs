using System.Web.Mvc;
using CloudSite.Models.User;
using CloudSite.Models.LogManager;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.AsyncFunctions;

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

            UserModel user = dbm.UserManager.GetUserData(ufl.UserEmailForLogin);
            if (user == null || !user.ConfirmedEmail)
                return View();

            ConvalidationUser cu = new ConvalidationUser(user);

            if (cu.CheckPasswordIsTheSame(ufl.UserPasswordForLogin, user.UserPassword))
            {
                Session["user_id"] = user._id.ToString();
                Session["userEmail"] = user.UserEmail;
                Session["userName"] = user.UserName;
                Session["login"] = 1;

                // Log
                LogMaster.WriteOnLog("user " + (string)Session["user_id"] + " is logged in");

                return RedirectToAction("Home", "Home");
            }

            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(UserModel user)
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

                // Log
                LogMaster.WriteOnLog("a new user have signin with username " + user.UserName);

                return Content("Abbiamo inviato un'email di conferma");
            }

            return View();
        }

        public ActionResult EmailAuth(string userId)
        {
            DBManager dbm = new DBManager();

            if (userId != null && dbm.UserManager.IsTheUserInTheDB(userId))
            {
                dbm.UserManager.ConfirmUserToMongoDB(userId);
                return RedirectToAction("Login", "Auth");
            }
            
            return Content("Errore");
        }
    }
}