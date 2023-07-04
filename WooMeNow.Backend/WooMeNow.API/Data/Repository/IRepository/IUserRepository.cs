using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository;

public interface IUserRepository
{
    void Update(User user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByUsernameAsync(string username);
    Task<MemberDto> GetMemberAsync(string username);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
}
