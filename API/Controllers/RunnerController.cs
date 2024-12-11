using API.Models.Forms;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class RunnerController(RunnerService rs) : ControllerBase {

    [HttpGet("{id}")]
    public IActionResult GetById(int id) {

      Runner? r = rs.GetById(id);
      return r is not null ? Ok(r) : NotFound();
    }

    [HttpGet("ByName")]
    public IActionResult GetByName([FromQuery] NameForm nf) {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      Runner? r = rs.GetByName(nf.Firstname!, nf.Lastname!);
      return r is not null ? Ok(r) : NotFound();
    }
  }
}
