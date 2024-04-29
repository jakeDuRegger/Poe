using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Poe.Helpers.Dependency_Injection;
using Poe.Models.API;
using Poe.Models.Document;

namespace Poe.ViewModels;



public partial class MainWindowViewModel : ObservableObject
{
    // Define an event for navigation
    public event Action<string, string> RequestNavigation;
    
    private readonly ConfigurationService _configService;
    
    private static readonly DataMuse _dataMuse = new DataMuse();
    
    /*
     * Settings Variables
     */
    private string? _lastFilePath;
    
    [ObservableProperty]
    private bool _isModified;

    [ObservableProperty] 
    private double _rtbHeight;

    
    public MainWindowViewModel()
    {
        // _configService = configService; // Service that allows access to user's json.
        
        
        // TODO: Figure out if this is the best way to handle. Potentially application level is better?
        // InitializeSettings(_configService); // Initialize 
        
        // if (configService.FirstTime)
        // {
        //     // Prompt introduction.
        //     IntroduceUser();
        // }
        //
        // // Check if user has a current open document with lastDocumentPath check.
        // var lastDocumentPath = _configService.LastDocumentPath;
        // if (!string.IsNullOrEmpty(lastDocumentPath))
        // {
        //     LoadLastDocumentOrNew(lastDocumentPath);
        // }
        // else
        // {
        //     // They don't have a recent document, so direct them to home.
        //     DirectHome();
        // }
    }

    private void InitializeSettings()
    {
        _lastFilePath = _configService.LastDocumentPath;
    }

    private void IntroduceUser()
    {
        throw new NotImplementedException();
    }

    private void DirectHome()
    {
        throw new NotImplementedException();
    }
    
    private bool AskUser()
    {
        throw new NotImplementedException();
    }
    

    private void LoadLastDocumentOrNew(string path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            OpenDocument(path); // Load last used document if it exists
        }
        else
        {
            // No last document or file doesn't exist anymore, start with a new document
            Document document; // Assuming this initializes a new blank document
            OnPropertyChanged(nameof(document));
        }
    }
    
    /*
     * Document view model stuff
     */
    
    [ObservableProperty] 
    private Document _document;
    
    
    [RelayCommand]
    private void NewDocument()
    {
        // Prompt to save current document if needed
        if (IsModified)
        {
            // Prompt question
            if (AskUser())
            {
                Save();
            }
        }
        
        // Handle the creation of the new document
        /*.
         * 
         * Functions:
         * - DocumentSettings(); // Asks for the user's preferred settings for the new document.
         * - CreateNewDocument(); // Handles all of the logic of initializing a new document.
         */
        
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
        // AskUser(); TODO: Ask user if they really want to exit.
        _lastFilePath = Document.CurrentFilePath; // Update last file path from user settings.
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
    
    // Method to get the current selected word
    private string GetCurrentSelectedWord()
    {
        return SelectedWord;
    }

    public void SetCurrentSelectedWord(string word)
    {
        SelectedWord = word;
    }

    private void OnRequestNavigation(string pageType, string word)
    {
        RequestNavigation?.Invoke(pageType, word);
    }
    
    public void GetDefinitionForView()
    {
        string word = SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Dictionary", word);
        }
    }

    public void GetThesaurusForView()
    {
        string word = SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Thesaurus", word);
        }
    }

    public void GetRhymeForView()
    {
        string word = SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Rhyming Dictionary", word);
        }
    }
    
    public Task<Dictionary<char, List<string>>> GetRhymeSchemeForView(List<string> text)
    {
        return _dataMuse.GetRhymeScheme(text);
    }

    /// <summary>
    /// Returns list of words from text using regex.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Task<List<string>> GetWordsFromText(string text)
    {
        return Task.Run(() =>
        {
            // Regex pattern to match words (sequences of word characters)
            var matches = Regex.Matches(text, @"[\w']+[^\p{P}\d]");
            
            // Convert matches to a list of words
            var words = new List<string>();
            foreach (Match match in matches)
            {
                words.Add(match.Value);
            }
            return words;
        });
    }
    
    /// <summary>
    /// Returns list of lowercase words from text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Task<List<string>> GetLowerCaseWordsFromText(string text) 
        // todo potentially rename function since it culls apostrophes as well...
    {
        return Task.Run(() =>
        {
            // Regex pattern to match words (sequences of word characters)
            var matches = Regex.Matches(text, @"[\w']+[^\p{P}\d]");

            
            // Convert matches to a list of words
            var words = new List<string>();
            foreach (Match match in matches)
            {
                // Convert to lower case and cull apostrophes.
                words.Add(match.Value.ToLower().Replace("'", "")); 
            }
            return words;
        });
    }
    
    /// <summary>
    /// Retrieves last word in line from given text.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns>List of strings in lowercase without punctuation.</returns>
     public Task<List<string>> GetEndWordsFromText(List<string> lines) 
     {
         return Task.Run(() =>
         {

             return lines
                 .Select(line =>
                 {
                     // Split line into words, remove empty entries, and trim punctuation from the last word
                     var lastWord = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                         .LastOrDefault();
                     lastWord = lastWord?.ToLower().Trim();

                     return string.IsNullOrEmpty(lastWord) ? "" : lastWord.TrimEnd('.', ',', ';', ':', '!', '?', '-', '\'', '—');;
                 })
                 .ToList();
         });
     }

    public Task<List<string>> GetLinesFromText(string text)
    {
        return Task.Run(() =>
        {
            // Normalize newline characters and split the text into lines
            var lines = text.Replace("\r\n", "\n").Split('\n');

            var lineList = lines.ToList();

            lineList.RemoveAll(string.IsNullOrWhiteSpace);
            
            return lineList;
        });
    }

    /// <summary>
    /// Checks if there are multiple words in a given string of text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public bool MultipleWords(string text)
    {
        // todo come back to this and potentially make this check more efficient
        // Split the string on whitespace characters and check the length of the array
        var words = text.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
        return words.Length > 1;
    }

    /*
     * Custom routed events
     */
    private void DiffAlgorithm()
    {
        
    }

}