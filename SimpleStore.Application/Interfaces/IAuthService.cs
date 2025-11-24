using SimpleStore.Application.DTOs.Token;
using SimpleStore.Application.DTOs.User;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
