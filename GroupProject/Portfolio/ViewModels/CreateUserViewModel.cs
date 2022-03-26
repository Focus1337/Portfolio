namespace Portfolio.ViewModels;

public class CreateUserViewModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime RegisterDate { get; set; }
}