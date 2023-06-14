using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data;
using WooMeNow.API.Models;

namespace WooMeNow.API.Controllers;

public class UsersController : BaseApiController
{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _db.Users.ToListAsync();
    }
     
    [HttpGet]
    [Route("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
