using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
  public class TokenForm {

    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }
  }
}
