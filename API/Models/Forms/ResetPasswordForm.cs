using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
  public class ResetPasswordForm {

    [Required]
    public int? UserId { get; set; }

    [Required]
    public string? ResetPasswordCode { get; set; }

    [Required]
    public string? NewPassword { get; set; }
  }
}
