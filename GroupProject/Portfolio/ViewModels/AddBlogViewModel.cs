using Portfolio.Entity;

namespace Portfolio.ViewModels;

public class AddBlogViewModel
{
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;

    public List<Tag> Tags { get; set; } = new(); // many to many
}