using System.Threading.Tasks;
using System.Web.Http;
using WebForms.Tools;

namespace WebForms.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<int> GetAsync(int id)
        {
            await HttpCall.GetWeatherForecastAsync();
            var client = new ServiceReference1.TestServiceClient();
            await client.GetDataAsync(100);
            return id;
        }
    }
}
