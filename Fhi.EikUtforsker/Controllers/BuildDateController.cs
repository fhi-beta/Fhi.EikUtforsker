using Microsoft.AspNetCore.Mvc;

namespace Fhi.EikUtforsker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildDateController : ControllerBase
{
    public static string BuildDate = "";

    public BuildDateController()
    {
    }

    [HttpGet]
    public async Task<ActionResult<BuildDateResponse>> Get()
    {
        return Ok(new BuildDateResponse(BuildDateController.BuildDate));
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