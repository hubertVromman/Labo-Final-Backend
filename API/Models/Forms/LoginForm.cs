﻿using System.ComponentModel.DataAnnotations;

namespace API.Models.Forms {
    public class LoginForm {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
