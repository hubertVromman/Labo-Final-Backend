﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.DTO {
    public class FullUser {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int RunnerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public int BirthYear { get; set; }
    }
}