namespace Portfolio.ViewModels;

public class ChangePasswordViewModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
}