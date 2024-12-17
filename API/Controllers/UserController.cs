using API.Mappers;
using API.Models.Forms;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
  [ApiController]
  public class UserController(UserService us) : ControllerBase {

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterForm form) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try {
        if (us.Register(
          email: form.Email.ToLower(),
          password: form.Password,
          firstname: form.Firstname,
          lastname: form.Lastname
        ))
          return Ok();
        else
          return BadRequest();
      } catch (Exception ex) {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginForm form) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

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
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

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
      string? tokenFromRequest = HttpContext.Request.Headers.Authorization;
      if (tokenFromRequest is null)
        return Unauthorized();
      string token = tokenFromRequest[7..];
      JwtSecurityToken jwt = new JwtSecurityToken(token);
      string email = jwt.Claims.First(x => x.Type == ClaimTypes.Email).Value;
      try {
        return Ok(us.GetByEmail(email));
      } catch (ArgumentException ex) {
        return NotFound(ex.Message);
      }
    }

    [Authorize("UserRequired")]
    [HttpPost("Anonymous")]
    public IActionResult ChangeAnonymous([FromBody] AnonymousForm af) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      string? tokenFromRequest = HttpContext.Request.Headers.Authorization;
      if (tokenFromRequest is null)
        return Unauthorized();
      string token = tokenFromRequest[7..];
      JwtSecurityToken jwt = new(token);
      int userId = int.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
      try {
        return Ok(us.ChangeAnonymous(userId, af.IsAnonymous));
      } catch (ArgumentException ex) {
        return NotFound(ex.Message);
      }
    }

    [Authorize("AdminRequired")]
    [HttpGet("All")]
    public IActionResult GetAll() {
      return Ok(us.GetAll());
    }

    [HttpPost("Activate")]
    public IActionResult Activate([FromBody] ActivationForm af) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (us.Activate(( (int)af.UserId! ), af.ActivationCode!))
        return Ok();
      else
        return BadRequest();
    }

    [HttpPost("ForgotPassword")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest fpr) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      us.ForgotPasswordRequest(fpr.Email);
      return Ok();
    }

    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword([FromBody] ResetPasswordForm rpf) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (us.ResetPassword((int)rpf.UserId!, rpf.ResetPasswordCode!, rpf.NewPassword!)) {
        return Ok();
      }
      return BadRequest();
    }

    [HttpHead("CheckEmail/{email}")]
    public IActionResult CheckEmail(string email) {
      FullUser? u = us.GetByEmail(email);
      return u is not null ? Ok() : NotFound();
    }

    [HttpHead("CheckName")]
    public IActionResult CheckEmail([FromQuery] NameForm nf) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      FullUser? u = us.GetByName(nf.Firstname!, nf.Lastname!);
      return u is not null ? Ok() : NotFound();
    }
  }
}
