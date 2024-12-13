using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
  public class AnonymousForm {

        [Required]
        public bool IsAnonymous { get; set; }
    }
}
