using API.Mappers;
using API.Models.Forms;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
                    email: form.Email.ToLower(),
                    password: form.Password
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

        [HttpGet("All")]
        public IActionResult GetAll() {
            return Ok(us.GetAll());
        }
    }
}
