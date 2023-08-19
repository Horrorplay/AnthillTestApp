using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnthillTest.API_Gateway.Controllers;
[Route("[controller]")]
[ApiController]
[EnableCors("CorsPolicy")]
public class GatewayController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}
