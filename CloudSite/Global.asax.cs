using CloudSite.Models.Log;
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

            /*My Methods*/
            Task whaitForCheckOrCreateLog = new Task(() => LogManager.CreateOrCheckIfExistFileLog());
            whaitForCheckOrCreateLog.Start();
            whaitForCheckOrCreateLog.Wait();
        }
    }
}
