using System.ComponentModel.DataAnnotations;

namespace WooMeNow.API.Models.DTOs;

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
