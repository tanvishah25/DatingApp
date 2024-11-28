﻿using System.Security.Claims;

namespace DatingApp.Extensions
{
    public static class ClaimsPrincipalExtenstions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cannot get UserName from token");
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get User Id from token"));
        }
    }
}
