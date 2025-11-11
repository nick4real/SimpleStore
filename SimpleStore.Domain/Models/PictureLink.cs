using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Models
{
    public class PictureLink
    {
        [Key]
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
