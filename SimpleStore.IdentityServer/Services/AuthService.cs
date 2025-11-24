using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleStore.Application.DTOs.Token;
using SimpleStore.Application.DTOs.User;
using SimpleStore.Application.Interfaces;
using SimpleStore.Domain.Entities;
using SimpleStore.IdentityServer.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace SimpleStore.IdentityServer.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserRegisterDto request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {
            if (String.IsNullOrWhiteSpace(request.Login))
                return null;

            User? user;
            if (request.Login.Contains('@'))
            {
                user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == request.Login);
            }
            else
            {
                user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == request.Login);
            }

            if (user is null) return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await RefreshSessionAsync(request.Guid, request.RefreshToken);
            if (user == null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
        }


        // Private methods to handle token generation and validation
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateAccessToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private string CreateAccessToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                issuer: configuration.GetValue<string>("AppSettings:Issuer")!,
                audience: configuration.GetValue<string>("AppSettings:Audience")!,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = CreateRefreshToken();

            var salt = RefreshTokenHasher.GenerateSalt();
            var refreshTokenHash = RefreshTokenHasher.HashToken(refreshToken, salt);

            var session = new Session
            {
                UserId = user.Id,
                RefreshTokenHash = Convert.ToBase64String(refreshTokenHash),
                Salt = Convert.ToBase64String(salt),
                Expires = DateTime.UtcNow.AddDays(14),
                IsRevoked = false
            };

            await context.Sessions.AddAsync(session);
            await context.SaveChangesAsync();

            return refreshToken;
        }

        private string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<User?> RefreshSessionAsync(Guid userId, string refreshToken)
        {
            var sessionList = await context.Sessions
                .Where(s => s.IsRevoked == false 
                && s.UserId == userId
                && DateTime.UtcNow <= s.Expires)
                .ToListAsync();
            if (sessionList == null) return null;

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var session = sessionList
                .FirstOrDefault(s => RefreshTokenHasher.VerifyHashToken(refreshToken, s.Salt, s.RefreshTokenHash));
            if (session == null) return null;

            session.IsRevoked = true;
            await context.SaveChangesAsync();

            return user;
        }
    }
}
