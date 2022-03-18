using System;
using System.Web;
using WcfService.Tools;

namespace WebForms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AppInstrumentatins.Configure();
        }

        void Application_End(object sender, EventArgs e)
        {
            AppInstrumentatins.Release();
        }
    }
}