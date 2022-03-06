namespace PortfolioAPI.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public Guid Secret { get; set; }
}