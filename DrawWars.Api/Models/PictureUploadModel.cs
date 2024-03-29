﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DrawWars.Api.Models
{
    public class PictureUploadModel
    {
        [Required]
        public Guid SessionID { get; set; }

        [Required]
        public Guid PlayerID { get; set; }

        [Required]
        public string Drawing { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public string Theme { get; set; }
    }
}
