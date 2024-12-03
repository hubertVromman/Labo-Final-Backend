﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ResultInfo
    {
        public int RaceId { get; set; }
        public int GeneralRank { get; set; }
        public string GeneralRankShown { get; set; }
        public TimeOnly? Time { get; set; }
        public int? GenderRank { get; set; }
        public int RunnerId { get; set; }
        public string? Gender { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}