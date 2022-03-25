using System.ComponentModel.DataAnnotations;

namespace Portfolio.ViewModels;

public class EditProfileViewModel
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string NewPassword { get; set; } = null!;
    [Required]
    public string OldPassword { get; set; } = null!;
}