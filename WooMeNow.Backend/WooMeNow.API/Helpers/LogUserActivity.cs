﻿using Microsoft.AspNetCore.Mvc.Filters;
using WooMeNow.API.Data.Repository;
using WooMeNow.API.Data.UnitOfWork;
using WooMeNow.API.Extensions;

namespace WooMeNow.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await uow.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await uow.CompleteAsync();
    }
}
