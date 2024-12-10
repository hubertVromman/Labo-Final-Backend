using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
    public class NameForm {

        [Required]
        public string? Firstname { get; set; }

        [Required]
        public string? Lastname { get; set; }
    }
}
