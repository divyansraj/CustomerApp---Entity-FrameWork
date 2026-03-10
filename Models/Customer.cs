using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Customer
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter valid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10-digit phone number")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [NotMapped]
    public string ConfirmPassword { get; set; }
    public List<Account> Accounts { get; set; }
}