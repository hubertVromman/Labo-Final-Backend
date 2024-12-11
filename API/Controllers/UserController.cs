using API.Mappers;
using API.Models.Forms;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class UserController(UserService us) : ControllerBase {

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterForm form) {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      try {
        us.Register(
            email: form.Email.ToLower(),
            password: form.Password,
            firstname: form.Firstname,
            lastname: form.Lastname
        );
        return Ok();
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginForm form) {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      try {
        return Ok(us.Login(
            email: form.Email!,
            password: form.Password!
        ));
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken([FromBody] TokenForm form) {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      try {
        return Ok(us.RefreshToken(
            form.ToBLL()
        ));
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }
    }

    [Authorize("UserRequired")]
    [HttpGet("Profile")]
    public IActionResult GetUserInfo() {
      string? tokenFromRequest = HttpContext.Request.Headers["Authorization"];
      if (tokenFromRequest is null)
        return Unauthorized();
      string token = tokenFromRequest.Substring(7, tokenFromRequest.Length - 7);
      JwtSecurityToken jwt = new JwtSecurityToken(token);
      string email = jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value;
      try {
        return Ok(us.GetByEmail(email));
      } catch (ArgumentException ex) {
        return NotFound(ex.Message);
      }
    }

    [HttpGet("All")]
    public IActionResult GetAll() {
      return Ok(us.GetAll());
    }

    [HttpHead("CheckEmail/{email}")]
    public IActionResult CheckEmail(string email) {
      FullUser? u = us.GetByEmail(email);
      return u is not null ? Ok() : NotFound();
    }

    [HttpHead("CheckName")]
    public IActionResult CheckEmail([FromQuery] NameForm nf) {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      FullUser? u = us.GetByName(nf.Firstname!, nf.Lastname!);
      return u is not null ? Ok() : NotFound();
    }
  }
}
