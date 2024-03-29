﻿
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace WebAPI
{
    [Index(nameof(Login), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public User()
        {
            Created = DateTime.UtcNow;
            Updated ??= Created;
        }

        public Guid Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime? Created { get; set; }

        [JsonIgnore]
        public DateTime? Updated { get; set; }
    }
}
