using WooMeNow.API.Models;

namespace WooMeNow.API.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
