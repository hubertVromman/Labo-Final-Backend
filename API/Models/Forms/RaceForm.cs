using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms
{
    public class RaceForm
    {
        [Required]
        public string? RaceName { get; set; }

        [Required]
        public string? Place { get; set; }

        [Required]
        public DateOnly? StartDate { get; set; }

        [Required]
        public string? RaceType { get; set; }

        [Required]
        public Decimal? Distance { get; set; }

        [Required]
        public Decimal? RealDistance { get; set; }

        [Required]
        public IFormFile? File { get; set; }
    }
}
