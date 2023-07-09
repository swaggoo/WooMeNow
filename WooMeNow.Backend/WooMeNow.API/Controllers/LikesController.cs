using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WooMeNow.API.Data;
using WooMeNow.API.Data.Repository;
using WooMeNow.API.Data.Repository.IRepository;
using WooMeNow.API.Data.UnitOfWork;
using WooMeNow.API.Extensions;
using WooMeNow.API.Helpers;
using WooMeNow.API.Models;

namespace WooMeNow.API.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public LikesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _uow.LikesRepository.GetUserWithLikesAsync(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _uow.LikesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already liked this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _uow.CompleteAsync()) return Ok();

        return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<UserLike>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await _uow.LikesRepository.GetUserLikesAsync(likesParams);

        Response.AddPaginationHeader(new PaginationHeader(
            users.CurrentPage, 
            users.PageSize, 
            users.TotalCount, 
            users.TotalPages));

        return Ok(users);
    }
}
