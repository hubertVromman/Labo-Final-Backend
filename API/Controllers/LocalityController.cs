using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LocalityController(LocalityService ls) : ControllerBase {
        [HttpGet]
        public IActionResult GetAll() {
            return Ok(ls.GetAll());
        }
    }
}
