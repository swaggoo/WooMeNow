﻿using WooMeNow.API.Helpers;
using WooMeNow.API.Models;
using WooMeNow.API.Models.DTOs;

namespace WooMeNow.API.Data.Repository.IRepository
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int targetUserId);
        Task<User> GetUserWithLikesAsync(int userId);
        Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams);
    }
}
