using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WcfService.Tools;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code,
    // svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs
    // at the Solution Explorer and start debugging.
    [ServiceBehavior(
        Namespace = "http://my.opentelemetry.net/",
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerCall,
        UseSynchronizationContext = false,
        Name = "TestService")]
    public class TestService : ITestService
    {
        public async Task<string> GetDataAsync(int value)
        {
            Counters.GetDataCalls.Add(1);
            using (var activity = Source.ActivitySource.StartActivity("GetData"))
            {
                activity?.SetTag("my.wcfservice.tag1", 1);

                await HttpCall.GetWeatherForecastAsync();
            }

            return string.Format("You entered: {0}", value);
        }

        public async Task<CompositeType> GetDataUsingDataContractAsync(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }

            Counters.GetDataCalls.Add(1);
            using (var activity = Source.ActivitySource.StartActivity("GetDataUsingDataContract"))
            {
                activity?.SetTag("my.wcfservice.tag1", 1);

                await HttpCall.GetWeatherForecastAsync();
            }

            return composite;
        }
    }
}
