using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Session.Utilities;

[AttributeUsage(AttributeTargets.Method)]
public class SessionAuthorization : Attribute, IAsyncActionFilter
{
    private readonly List<string> roles = new();

    public SessionAuthorization()
    {
        roles.Add(SessionConstants.AdminRole);
        roles.Add(SessionConstants.UserRole);
    }

    public SessionAuthorization(params string[] sessionRoles)
    {
        this.roles.AddRange(sessionRoles);
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var username = context.HttpContext.Session.GetString(SessionConstants.UsernameProperty);
        var role = context.HttpContext.Session.GetString(SessionConstants.RoleProperty);

        if (string.IsNullOrEmpty(username) || this.roles.Contains(role) == false)
        {
            Log.Warning("Client ist nicht Authorisiert. {Username} {Rolle}", username, role);
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                            { "Controller", "Home" },
                            { "Action", "Index" } });

            return;
        }

        await next();
    }
}
