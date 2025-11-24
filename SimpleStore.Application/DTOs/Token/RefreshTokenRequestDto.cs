namespace SimpleStore.Application.DTOs.Token
{
    public class RefreshTokenRequestDto
    {
        public Guid Guid { get; set; }
        public required string RefreshToken { get; set; }
    }
}
