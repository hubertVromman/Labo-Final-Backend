using API.Models.Forms;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class ResultController(ResultService rs) : ControllerBase {

    [HttpGet("ByRunner/{runnerId}")]
    public IActionResult GetByRunnerId(int runnerId) {
      IEnumerable<Result> results = rs.GetByRunnerId(runnerId);
      return Ok(results);
    }

    [HttpGet("ByRace/{raceId}")]
    public IActionResult GetByRaceId(int raceId, [FromQuery] PaginationForm pf) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      ObjectList<Result> results = rs.GetByRaceId(raceId, (int)pf.Offset!, (int)pf.Limit!);
      return Ok(results);
    }
  }
}
