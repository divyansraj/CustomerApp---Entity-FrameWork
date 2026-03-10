using System.ComponentModel.DataAnnotations;

namespace Customerapp.Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter valid email format")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
