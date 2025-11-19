using NeonTest.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NeonTest.Filters;

public class Authorization : ActionFilterAttribute
{
  private readonly JwtService _jwt;
  
  public Authorization(JwtService jwt)
  {
    _jwt = jwt;
  }
  public override async void OnActionExecuting(ActionExecutingContext context)
  {
    var id = GetId(context);
    if (id == "-1")
      context.Result = new ContentResult { Content = "Unauthorized (no id)" };
    string token = context.HttpContext.Request.Headers.Authorization.ToString().Split(" ")[1];

    if (_jwt.ValidateToken(token) == false)
      context.Result = new ContentResult { Content = "Unauthorized (invalid token)" };
    else
    {

      var tokenHandler = new JwtSecurityTokenHandler();
      var claims = tokenHandler.ReadJwtToken(token).Claims;
      foreach (var claim in claims)
      {
        if (claim.Type == ClaimTypes.NameIdentifier)
        {
          if (claim.Value != id)
          {
            context.Result = new ContentResult { Content = "Unauthorized (wrong id) "};
            break;
          }
        }
      }
    }
    
  }

  public override void OnActionExecuted(ActionExecutedContext context)
  {}

  private string GetId(ActionExecutingContext c)
  {
    if (c.RouteData.Values.TryGetValue("id", out var id))
    {
      return id.ToString();
    } else return "-1";
  }
}
