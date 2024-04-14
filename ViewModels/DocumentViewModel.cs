using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Poe.Models;
using System.Collections.ObjectModel;

namespace Poe.ViewModels;

public partial class DocumentViewModel : ObservableObject
{
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
