﻿using System.Security.Claims;
using WooMeNow.API.Models;

namespace WooMeNow.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.Name)?.Value;

        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.Parse(username);
    }
}
