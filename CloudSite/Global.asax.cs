using CloudSite.Models.LogManager;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CloudSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /* My Methods for Log */
            Task whaitForCheckOrCreateLog = new Task(() => LogMaster.CreateOrCheckIfExistFileLog());
            whaitForCheckOrCreateLog.Start();
            whaitForCheckOrCreateLog.Wait();
        }
    }
}
