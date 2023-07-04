using AutoMapper;
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
    private readonly IMapper _mapper;

    public AccountController(ApplicationDbContext db, ITokenService tokenService, IMapper mapper)
    {
        _db = db;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = _mapper.Map<User>(registerDto);

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _db.Users.Include(u => u.Photos).FirstOrDefaultAsync(x => 
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
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _db.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}
