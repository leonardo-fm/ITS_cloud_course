using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models;
using CloudSite.Models.ComputerVision;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            ViewBag.user_id = Session["user_id"];
            ViewBag.userEmail = Session["userEmail"];
            ViewBag.userName = Session["userName"];

            return View();
        }
    }
}