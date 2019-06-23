using System;

namespace IdentityServer.Models
{
    public class RefreshToken
    {
        public string RefreshTokenId { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }
        public DateTime? DateExpires { get; set; }

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
