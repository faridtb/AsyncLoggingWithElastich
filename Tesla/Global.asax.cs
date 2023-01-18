using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Tesla
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebApiApplication)); 
        protected void Application_Start()
        {
            XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.Info("Salam Agasif Muellim");
            
            

        }
    }
}
