using System;

namespace IdentityServer.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
