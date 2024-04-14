using CommunityToolkit.Mvvm.ComponentModel;


namespace Poe.Models.Document;

/**
 * Model of document which includes pages, bookmarks, sticky notes, and user functionality
 * such as saving, loading, and printing documents.
 */
public class Document : ObservableObject
{
    public Document()
    {
     
    }

    /**
     * Loads the document to pass to DocumentViewModel
     */
    public void LoadDocument(string filePath)
    {
     
    }

    /**
     * Saves the document
     */
    public void SaveDocument()
    {
    }

    /**
    * Prints the document
    */
    public void PrintDocument()
    {
    }
    
    /**
     * Convert XAML text into HTML
     */
    public string ConvertXamlToHtml(string xamlContent)
    {
     // Basic conversion: you will need to expand this based on your XAML structure
     xamlContent = xamlContent.Replace("<Paragraph>", "<p>");
     xamlContent = xamlContent.Replace("</Paragraph>", "</p>");
     xamlContent = xamlContent.Replace("<Run>", "<span>");
     xamlContent = xamlContent.Replace("</Run>", "</span>");
     // Add more replacements as needed for other elements like Bold, Italic, etc.
     return xamlContent;
    }
}