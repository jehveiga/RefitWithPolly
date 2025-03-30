using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebApi.ExternalApis.WebApiFail;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IWebApiFail _webApiFail;

        public TestController(ILogger<TestController> logger, IWebApiFail webApiFail)
        {
            _webApiFail = webApiFail;
        }

        [HttpGet(Name = "GetTest")]
        public async Task<string> GetTest()
        {
            Log.Information("GetTest called");
            return await _webApiFail.GetTestAsync();
        }
    }
}