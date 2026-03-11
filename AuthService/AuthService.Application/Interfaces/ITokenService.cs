using System.Threading.Tasks;
using AuthService.Application.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateTokenAsync(ApplicationUser user);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        string GenerateRefreshToken();
    }
}
