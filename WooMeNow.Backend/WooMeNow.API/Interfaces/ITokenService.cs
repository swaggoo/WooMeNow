using WooMeNow.API.Models;

namespace WooMeNow.API.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}
