using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WooMeNow.API.Data;
using WooMeNow.API.Interfaces;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ApplicationDbContext _db;
    private readonly ITokenService _tokenService;

    public AccountController(ApplicationDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;

    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new User()
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserDto 
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => 
            x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _db.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}
