using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Race
    {
        public string RaceName { get; set; }
        public string Place { get; set; }
        public DateOnly StartDate { get; set; }
        public string RaceType { get; set; }
        public Decimal Distance { get; set; }
        public Decimal RealDistance { get; set; }
    }
}