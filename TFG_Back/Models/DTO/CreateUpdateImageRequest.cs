﻿namespace TFG_Back.Models.DTO
{
    public class CreateUpdateImageRequest
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
