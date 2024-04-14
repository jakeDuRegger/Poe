namespace Poe.Models;

/**
 * Model for creating a bookmark for the user to add to the document.
 */
public class Bookmark(int position, string name, string color, string information)
{
    public int Position { get; set; } = position;
    public string Name { get; set; } = name;
    public string Color { get; set; } = color;
    public string Information { get; set; } = information;
    public DateTime Time { get; set; }
    // Todo: Potentially a notify feature to tell the user when to come back to the bookmark.
    
}