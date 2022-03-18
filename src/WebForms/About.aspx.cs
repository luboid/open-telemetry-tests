using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForms.Tools;

namespace WebForms
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Counters.NumberAbout.Add(1);
            using (var activity = Source.ActivitySource.StartActivity("my.about.activity"))
            {
                activity?.AddTag("my.about.tag", 100);
                HttpCall.GetWeatherForecast();
                HttpCall.RequestGoogleHomPageViaHttpClient();
                HttpCall.RequestGoogleHomPageViaHttpWebRequestLegacySync();

                var client = new ServiceReference1.TestServiceClient();
                client.GetData(100);
            }
        }
    }
}