using System.ComponentModel.DataAnnotations;

namespace WooMeNow.API.Models.DTOs;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; }
}
