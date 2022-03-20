using System.ComponentModel.DataAnnotations;
namespace Portfolio.ViewModels;

public class RequestViewModel
{
     [Required]
    public string Name { get; set; } = null!;
     [Required]
    public string Email { get; set; } = null!;
     [Required]
    public string Subject { get; set; } = null!;
     [Required]
    public string Message { get; set; } = null!;
}