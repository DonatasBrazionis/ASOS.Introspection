using System;
using System.Threading.Tasks;
using IdentityServer.Models;

namespace IdentityServer.Contracts
{
    public interface IDatabaseProvider
    {
        Task<bool> IsRefreshTokenExists(string refreshToken);
        Task CreateRefreshToken(string userId, string refreshToken, string remoteIpAddress, string userAgent, TimeSpan? refreshTokenLifetime);
        Task DeleteRefreshToken(string refreshToken);

        Task<User> GetUserByEmail(string email);

        Task<bool> IsClientExists(string clientId, string clientSecret);
    }
}
