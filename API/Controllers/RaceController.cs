using API.Mappers;
using API.Models.Forms;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class RaceController(RaceService rs) : ControllerBase {

    [HttpPost()]
    [RequestFormLimits(MultipartBodyLengthLimit = 20_000_000, ValueLengthLimit = 20_000_000)]
    public ActionResult AddRace(RaceForm rf) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      Race race;
      try {
        race = rs.AddRaceIfNotExist(rf.ToDomain());
      } catch (Exception ex) {
        return BadRequest(new { error = ex.Message });
      }

      string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

      if (!Directory.Exists(pathToSave))
        Directory.CreateDirectory(pathToSave);

      string fullPath = Path.Combine(pathToSave, $"{rf.RaceName} {rf.Distance}km {((DateOnly)rf.StartDate!).Year}.pdf");
      //if (System.IO.File.Exists(fullPath)) {
      //  return BadRequest(new { error = "Race already added" });
      //}
      using (FileStream stream = new(fullPath, FileMode.Create)) {
        rf.File!.CopyTo(stream);
      }

      rs.ParsePDF(fullPath, race);

      return Ok();
    }

    [HttpGet("ByDate")]
    public ActionResult GetByDate([FromQuery] PaginationForm pf) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(rs.GetByDate((int)pf.Offset!, (int)pf.Limit!));
    }

    [HttpGet("{id}")]
    public ActionResult GetByDate([FromRoute] int id) {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      return Ok(rs.GetById(id));
    }

    [HttpGet("Search/{query}")]
    public IActionResult Search(string query) {

      return Ok(rs.Search(query));
    }
  }
}
