using Microsoft.AspNetCore.Mvc;
using WooMeNow.API.Helpers;

namespace WooMeNow.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    
}
