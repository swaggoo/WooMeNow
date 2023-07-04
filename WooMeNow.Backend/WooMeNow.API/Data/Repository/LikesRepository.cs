using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data.Repository.IRepository;
using WooMeNow.API.Extensions;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository
{
    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationDbContext _db;

        public LikesRepository(ApplicationDbContext db) 
        {
            _db = db;
        }


        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _db.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _db.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = _db.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Id = user.Id,
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                City = user.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageSize, likesParams.PageSize);

        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            return await _db.Users
                .Include(user => user.LikedUsers)
                .FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}
