using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
    public class PaginationForm {

        [Required]
        public int? Offset { get; set; }

        [Required]
        public int? Limit { get; set; }
    }
}
