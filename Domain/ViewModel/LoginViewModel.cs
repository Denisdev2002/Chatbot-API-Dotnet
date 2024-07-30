using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is mandatory.")]
        [EmailAddress(ErrorMessage = "The email provided is not valid.")]
        public string login { get; set; }

        [Required(ErrorMessage = "Password is mandatory.")]
        public string senha { get; set; }
    }
}