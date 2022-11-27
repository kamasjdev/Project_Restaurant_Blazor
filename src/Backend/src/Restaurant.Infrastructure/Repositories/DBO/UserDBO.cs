﻿namespace Restaurant.Infrastructure.Repositories.DBO
{
    internal sealed class UserDBO
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
