﻿using System.ComponentModel.DataAnnotations;

namespace Chat.Models
{
    public class MessageViewModel
    {
        [Required]
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public string From { get; set; }
        [Required]
        public string Room { get; set; }
        public string Avatar { get; set; }
    }
}
