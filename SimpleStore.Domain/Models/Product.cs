﻿using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Models
{
    public class Product
    {
        [Key]
        public Guid Guid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public float Price { get; set; }
    }
}
