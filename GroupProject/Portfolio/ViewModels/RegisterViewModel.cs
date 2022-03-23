using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    // [Required]
    // [Display(Name = "Год рождения")]
    // public int Year { get; set; }
 
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
 
    [Required]
    [Compare("Password", ErrorMessage = "Passwords are different!")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string PasswordConfirm { get; set; }
}