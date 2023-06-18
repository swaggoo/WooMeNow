using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WooMeNow.API.Data;
using WooMeNow.API.Models;

namespace WooMeNow.API.Controllers;

public class BuggyController : BaseApiController
{
    private ApplicationDbContext _db;

    public BuggyController(ApplicationDbContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<User> GetNotFound()
    {
        var thing = _db.Users.FirstOrDefault(user => user.Id == -1);

        if (thing == null) return NotFound();

        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _db.Users.FirstOrDefault(u => u.Id == -1);

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }
}
