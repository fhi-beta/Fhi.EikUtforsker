using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.EikUtforsker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildDateController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<BuildDateResponse>> Get() 
        {
            return Ok(new BuildDateResponse(Startup.BuildDate));
        }
    }

    public class BuildDateResponse
    {
        public BuildDateResponse(string buildDate)
        {
            BuildDate = buildDate;
        }
        public string BuildDate { get; set; }
    }
}