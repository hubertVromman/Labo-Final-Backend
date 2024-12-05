using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController(ResultService rs) : ControllerBase {

        [HttpGet("{runnerId}")]
        public IActionResult GetByRunnerId(int runnerId) {
            IEnumerable<Result> results = rs.GetByRunnerId(runnerId);
            return Ok(results);
        }
    }
}
