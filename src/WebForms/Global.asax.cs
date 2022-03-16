using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using WebForms.App_Code;

namespace WebForms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AppInstrumentatins.Configure();
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_End(object sender, EventArgs e)
        {
            AppInstrumentatins.Release();
        }
    }
}