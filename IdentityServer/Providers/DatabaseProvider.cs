using System;
using System.Threading.Tasks;
using IdentityServer.Contracts;
using IdentityServer.Models;

namespace IdentityServer.Providers
{
    public class DatabaseProvider : IDatabaseProvider
    {
        public async Task CreateRefreshToken(string userId, string refreshToken, string remoteIpAddress, string userAgent, TimeSpan? refreshTokenLifetime)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException();
            }

            var entity = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                DateExpires = DateTime.UtcNow + refreshTokenLifetime,
                IpAddress = remoteIpAddress,
                UserAgent = userAgent,
                DateCreated = DateTime.UtcNow
            };

            // TODO: Save `entity` to DB.
            await Task.Delay(10);
        }

        public async Task DeleteRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException();
            }

            // TODO: Remove `refreshToken` from DB.
            await Task.Delay(10);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException();
            }

            // TODO: Get `User` by `email` from DB.
            await Task.Delay(10);
            return new User
            {
                UserId = Guid.NewGuid().ToString(),
                    FullName = "John Snow",
                    Email = "admin@mail.com",
                    // Password is `admin`
                    PasswordHash = "$2b$10$X8hN6txN24yNUDm47xF0Zefv.aeI.S6teHvLa6UrzPoKSqK7OfvSy",
                    DateCreated = DateTime.UtcNow
            };
        }

        public async Task<bool> IsClientExists(string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException();
            }

            // TODO: Try to get `ResourceServerClient` by `clientId` and `clientSecret` from DB.
            await Task.Delay(10);
            return clientId == "5035f951-f7bb-459d-b196-bb212292bb4d" && clientSecret == "89e43125-d963-4694-b770-096795a6e1e1";
        }

        public async Task<bool> IsRefreshTokenExists(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException();
            }

            // TODO: Try to get `RefreshToken` by `refreshToken` and `RefreshToken.DateExpires > DateTime.UtcNow` from DB.
            await Task.Delay(10);
            return true;
        }
    }
}
