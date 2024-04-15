using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Poe.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using DocumentFormat.OpenXml.CustomProperties;
using Microsoft.Win32;
using Poe.Models.Document;

namespace Poe.ViewModels;


/**
 * Note: To call a command / property in the MainWindow use: DocumentVM.'Binding'
 */
public partial class DocumentViewModel : ObservableObject
{
    [ObservableProperty] 
    private Document _document;
    
    public DocumentViewModel()
    {
        Document = new Document();
        LoadLastDocumentOrNew();
    }
    
    private void LoadLastDocumentOrNew()
    {
        var lastPath = config.LastDocumentPath;
        if (!string.IsNullOrEmpty(lastPath) && File.Exists(lastPath))
        {
            OpenDocument(lastPath); // Load last used document if it exists
        }
        else
        {
            // No last document or file doesn't exist anymore, start with a new document
            Document = new Document(); // Assuming this initializes a new blank document
            OnPropertyChanged(nameof(Document));
        }
    }
    
    [RelayCommand]
    private void NewDocument()
    {
        // Prompt to save current document if needed
        if (Document.IsModified) // Assuming IsModified is a property that tracks changes
        {
            var result = MessageBox.Show("Do you want to save changes to your current document?", "Save Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                return; // User canceled the new document operation
            }
            if (result == MessageBoxResult.Yes)
            {
                Save();
            }
        }

        Document = new Document(); // Clear current document or initialize a new one
        Document.CurrentFilePath = null; // Reset file path
        OnPropertyChanged(nameof(Document));
    }
    
    private void OpenDocument(string filePath)
    {
        try
        {
            // Assuming your Document model has a method to load from a file
            Document.LoadFile(filePath);
            Document.CurrentFilePath = filePath;  // Update the current file path
            OnPropertyChanged(nameof(Document));  // Notify UI of changes
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    [RelayCommand]
    private void Save()
    {
        if (!string.IsNullOrEmpty(Document.CurrentFilePath))
        {
            Document?.SaveFile(Document.CurrentFilePath);  // Save to the existing file path
        }
        else
        {
            SaveAs();  // No current file path, invoke Save As
        }
    }

    [RelayCommand]
    private void SaveAs()
    {
        var filePath = GetFilePathFromSaveDialog();
        if (!string.IsNullOrEmpty(filePath))
        {
            Document?.SaveFile(filePath);
            if (Document != null) Document.CurrentFilePath = filePath; // Update the file path after saving
        }
        else
        {
            MessageBox.Show("Save operation was cancelled or no file path was selected.", "Save Cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void Open()
    {
        var filePath = GetFilePathFromOpenDialog();
        if (!string.IsNullOrEmpty(filePath))
        {
            Document.LoadFile(filePath);
            Document.CurrentFilePath = filePath;  // Store the path of the opened file
            OnPropertyChanged(nameof(Document));
        }
    }

    [RelayCommand]
    private void Exit()
    {
        Save();
        // Optionally save current file path to settings
        config.LastDocumentPath = Document.CurrentFilePath;
        config.Default.Save();
        Environment.Exit(0);  // Close the application
    }


    private string GetFilePathFromSaveDialog()
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Rich Text Format (*.rtf)|*.rtf";
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return null;
    }

    private string GetFilePathFromOpenDialog()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        // Configure dialog properties
        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }
        return null; // or handle as required
    }

    
    [ObservableProperty]
    private string _selectedWord;
    
    public string FormattedSelectedWordDefinition => string.IsNullOrEmpty(SelectedWord) ? "Find definition" : $"Define '{SelectedWord}'";
    public string FormattedSelectedWordRhyme => string.IsNullOrEmpty(SelectedWord) ? "Find rhymes" : $"Rhymes with '{SelectedWord}'";
    public string FormattedSelectedWordThesaurus => string.IsNullOrEmpty(SelectedWord) ? "Find synonyms and antonyms" : $"Lookup synonyms and antonyms for '{SelectedWord}'";

    // Whenever _selectedWord changes, it updates the properties in the view.
    partial void OnSelectedWordChanged(string value)
    {
        // Notify the UI that these dependent properties need to be re-evaluated
        OnPropertyChanged(nameof(FormattedSelectedWordDefinition));
        OnPropertyChanged(nameof(FormattedSelectedWordRhyme));
        OnPropertyChanged(nameof(FormattedSelectedWordThesaurus));
        // I can add anything here / any bindings and it would update according to the selected word!
    }
    
}



    // public ObservableCollection<Bookmark> Bookmarks { get; } = [];
    //
    // [RelayCommand]
    // public void AddBookmark(int position, string name, string color, string information)
    // {
    //     var bookmark = new Bookmark(position, name, color, information);
    //     Bookmarks.Add(bookmark);
    //     OnPropertyChanged(nameof(Bookmarks));
    // }
