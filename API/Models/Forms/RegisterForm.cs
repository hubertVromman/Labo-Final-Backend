using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
  public class RegisterForm {

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    public string Lastname { get; set; }
  }
}
