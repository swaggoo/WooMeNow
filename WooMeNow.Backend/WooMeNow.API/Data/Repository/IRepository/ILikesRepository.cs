using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository.IRepository
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<User> GetUserWithLikes(int userId);
        Task<IEnumerable<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
