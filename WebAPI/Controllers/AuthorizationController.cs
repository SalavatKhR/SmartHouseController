using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthorizationController: ControllerBase
{
    [HttpGet("test")]
    public IActionResult TestAction(string data)
    {
        return Ok(data);
    }
}