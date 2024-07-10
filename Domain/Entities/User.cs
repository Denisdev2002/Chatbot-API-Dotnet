
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public Role Role { get; set; }
        public int countSession { get; set; }
    }
}