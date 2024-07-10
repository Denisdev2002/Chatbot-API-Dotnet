
using Domain.Entities;
using System.Data;

namespace Domain.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Number { get; set; }
        public Role Role { get; set; }
    }
}