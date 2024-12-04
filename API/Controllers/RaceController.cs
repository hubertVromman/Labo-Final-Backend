using API.Mappers;
using API.Models.Forms;
using API.Tools;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RaceController(RaceService rs) : ControllerBase {

        [HttpPost()]
        [RequestFormLimits(MultipartBodyLengthLimit = 20_000_000, ValueLengthLimit = 20_000_000)]
        public ActionResult AddRace(RaceForm rf) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int raceId = rs.AddRace(rf.ToDomain());

            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            string fullPath = Path.Combine(pathToSave, $"{rf.RaceName} {((DateOnly)rf.StartDate!).Year} {rf.Distance}km.pdf");
            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                rf.File!.CopyTo(stream);
            }

            rs.ParsePDF(fullPath, raceId);

            return Ok();
        }
    }
}
