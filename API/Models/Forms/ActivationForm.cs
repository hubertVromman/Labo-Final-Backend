using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
  public class ActivationForm {

    [Required]
    public string? ActivationCode { get; set; }

    [Required]
    public int? UserId { get; set; }
  }
}
