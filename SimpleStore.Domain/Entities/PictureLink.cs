using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Entities
{
    public class PictureLink
    {
        [Key]
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
