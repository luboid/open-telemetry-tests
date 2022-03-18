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
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var activity = Source.ActivitySource.StartActivity("my.contact.activity"))
            {
                activity?.AddTag("my.contact.tag", 100);
                HttpCall.GetWeatherForecast();
            }
        }
    }
}