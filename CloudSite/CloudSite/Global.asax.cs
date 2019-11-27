using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CloudSite.Models.Log;

namespace CloudSite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            /*My Methods*/
            Task whaitForCheckOrCreateLog = new Task(() => LogManager.CreateOrCheckIfExistFileLog());
            whaitForCheckOrCreateLog.Start();
            whaitForCheckOrCreateLog.Wait();
        }
    }
}
