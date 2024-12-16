using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class MailController(MailService ms) : ControllerBase {

    [HttpPost]
    public bool SendMail(MailData Mail_Data) {
      return ms.SendMail(Mail_Data);
    }
  }
}
