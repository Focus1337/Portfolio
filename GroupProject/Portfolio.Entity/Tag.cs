namespace Portfolio.Entity;

public class Tag
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;

    public List<Post> Posts { get; set; }= new();
}