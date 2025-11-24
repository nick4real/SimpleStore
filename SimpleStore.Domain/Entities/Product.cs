using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Guid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public decimal Price { get; set; }
        public List<PictureLink> Pictures { get; set; } = new();
    }
}
