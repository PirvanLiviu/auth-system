using NeonTest.Data;
using NeonTest.Dtos;
using NeonTest.Models;
using NeonTest.Services;
using NeonTest.Filters;
using static NeonTest.Services.PasswordService;
using Microsoft.AspNetCore.Mvc;

namespace NeonTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly NeonDbContext _context;
  private readonly JwtService _jwt;

  public UserController(NeonDbContext context, JwtService jwt)
  {
    _context = context;
    _jwt = jwt;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
  {
    var newUser = new User 
      {
        Username = registerUser.Username,
        Email = registerUser.Email,
        Password = HashPassword(registerUser.Password)
      };
    var res = _context.User.Add(newUser).Entity;
    await _context.SaveChangesAsync();

    return Ok(new { user = new User { Id = res.Id, Username = res.Username, Email = res.Email, Password = res.Password }, token = _jwt.GenerateToken(res.Id, res.Username, res.Email) });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
  {
    var res = _context.User.Where<User>(u => u.Username == loginUser.Username).First();
    if (VerifyPassword(loginUser.Password, res.Password))
    {
      return Ok(new { message = "Log in successful", token = _jwt.GenerateToken(res.Id, res.Username, res.Email), user = new User 
          {
          Id = res.Id,
          Username = res.Username,
          Email = res.Email,
          Password = res.Password
          }});
    }

    return BadRequest(new { message = "Incorrect credentials" });
  }

  [TypeFilter(typeof(Authorization))]
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetUser([FromRoute] int id, [FromHeader] string authorization)
  {
    var res = await _context.User.FindAsync(id);
    if (res == null)
      return BadRequest(new { message = "No user found" });
    return Ok(new { username = res.Username, email = res.Email, id = res.Id });
  }
}
