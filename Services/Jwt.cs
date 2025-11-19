using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace NeonTest.Services;

public class JwtService
{
  private readonly IConfiguration _config;

  public JwtService(IConfiguration config)
  {
    _config = config;
  }

  public string GenerateToken(int userid, string username, string email)
  {
    var claims = new Claim[]
    {
      new Claim(ClaimTypes.NameIdentifier, userid.ToString()),
      new Claim(ClaimTypes.Name, username),
      new Claim(ClaimTypes.Email, email),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:signingkey"]));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["JWT:issuer"],
        audience: _config["JWT:audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
        );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public bool ValidateToken(string token)
  {
    var handler = new JwtSecurityTokenHandler();
    var parameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = _config["JWT:issuer"],
      ValidAudience = _config["JWT:audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:signingkey"]))
    };

    SecurityToken validatedToken;
    try
    {
      var principal = handler.ValidateToken(token, parameters, out validatedToken);
      return true;
    } catch
    {
      return false;
    }
  }
}
