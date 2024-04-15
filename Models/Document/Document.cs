
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;



namespace Poe.Models.Document;

/**
 * Model of document which includes pages, bookmarks, sticky notes, and user functionality
 * such as saving, loading, and printing documents.
 */
public partial class Document : ObservableObject
{
    [ObservableProperty] 
    private string _content;
    
    [ObservableProperty]
    private string currentFilePath;


    public Document()
    {
     App.   
    }

    /**
     * Loads the document to pass to DocumentViewModel
     * <param name="filePath"></param>
     */
    public void LoadFile(string filePath)
    {
        try
        {
            Content = File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void SaveFile(string filePath)
    {
        try
        {
            File.WriteAllText(filePath, Content);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    public void AutoSave(string filePath)
    {
        // Look up logic to find the current file being used...
    }

}