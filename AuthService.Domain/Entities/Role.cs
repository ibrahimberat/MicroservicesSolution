using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
    }
}