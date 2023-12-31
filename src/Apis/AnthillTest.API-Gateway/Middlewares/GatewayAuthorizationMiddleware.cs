﻿using Ocelot.Authorization;
using Ocelot.Middleware;
using System.Linq;
using System.Security.Claims;

namespace AnthillTest.API_Gateway.Middlewares;

public class GatewayAuthorizationMiddleware
{
    public static async Task Authorize(HttpContext httpContext, Func<Task> next)
    {
        if (ValidateRole(httpContext) && ValidateScope(httpContext))
            await next.Invoke();
        else
        {
            httpContext.Response.StatusCode = 403;
            httpContext.Items.SetError(new UnauthorizedError($"Fail to authorize"));
        }
    }

    private static bool ValidateScope(HttpContext httpContext)
    {
        string scopeName = "scope";

        var downstreamRoute = httpContext.Items.DownstreamRoute();
        var listOfRequiredScopes = downstreamRoute.AuthenticationOptions.AllowedScopes;
        if (listOfRequiredScopes == null || listOfRequiredScopes.Count == 0) return true;
        var userClaimsPrincipals = httpContext.User.Claims.ToArray<Claim>();
        List<string> listOfClaimTypes = new List<string>();
        List<string> listOfScopes = new List<string>();
        foreach (var userClaim in userClaimsPrincipals)
        {
            listOfClaimTypes.Add(userClaim.Type);
            if (userClaim.Type == scopeName)
                listOfScopes.Add(userClaim.Value);
        }

        if (!listOfClaimTypes.Contains(scopeName))
            return false;

        foreach (var scope in listOfScopes)
        {
            if (listOfRequiredScopes.Contains(scope))
                return true;
        }

        return false;
    }

    private static bool ValidateRole(HttpContext ctx)
    {
        var downStreamRoute = ctx.Items.DownstreamRoute();
        if (downStreamRoute.AuthenticationOptions.AuthenticationProviderKey == null)
            return true;

        Claim[] userClaims = ctx.User.Claims.ToArray<Claim>();

        Dictionary<string, string> requiredAuthorizationClaims = downStreamRoute.RouteClaimsRequirement;
        if (requiredAuthorizationClaims.Count() < 1)
        {
            return true;
        }

        foreach (KeyValuePair<string, string> requiredAuthorizationClaim in requiredAuthorizationClaims)
        {
            if (ValidateIfStringIsRole(requiredAuthorizationClaim.Key))
            {
                foreach (var requiredClaimValue in requiredAuthorizationClaim.Value.Split(","))
                {
                    foreach (Claim userClaim in userClaims)
                    {
                        if (ValidateIfStringIsRole(userClaim.Type) && requiredClaimValue.Equals(userClaim.Value))
                            return true;
                    }
                }
            }
        }
        return false;
    }

    private static bool ValidateIfStringIsRole(string role)
    {
        return role.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") || role.Equals("Role") ||
               role.Equals("role");
    }
}
