using Microsoft.AspNetCore.Identity;

namespace Portfolio.Entity;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime RegisterDate { get; set; }
}