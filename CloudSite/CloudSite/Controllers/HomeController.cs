﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudSite.Models.ComputerVision;

namespace CloudSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                Task t1 = new Task(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(file));
                t1.Start();

                return Content("Immagine caricata");
            }

            return View();
        }
    }
}