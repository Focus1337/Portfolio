namespace Portfolio.Entity;

public class Post
{
    public string PostId { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;

    public string AuthorId { get; set; } = null!;
    public User Author { get; set; } = null!; // many to one
    public List<Tag> Tags { get; set; } = new(); // many to many
}