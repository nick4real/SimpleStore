using System.ComponentModel.DataAnnotations;

namespace SimpleStore.Domain.Entities
{
    public class Session
    {
        [Key]
        public uint Id { get; set; }
        public required Guid UserId { get; set; }
        public required string RefreshTokenHash { get; set; }
        public required string Salt { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string Device { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
