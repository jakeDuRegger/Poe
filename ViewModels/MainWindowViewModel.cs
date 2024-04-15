using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using Material.Icons.WPF;
using Poe.Views;

namespace Poe.ViewModels;



public partial class MainWindowViewModel : ObservableObject
{
    public DocumentViewModel DocumentVM { get; }
    
    // Define an event for navigation
    public event Action<string, string> RequestNavigation;

    public MainWindowViewModel()
    {
        DocumentVM = new DocumentViewModel();
    }
    
    // Method to get the current selected word from the DocumentViewModel
    private string GetCurrentSelectedWord()
    {
        return DocumentVM.SelectedWord;
    }
    
    private void OnRequestNavigation(string pageType, string word)
    {
        RequestNavigation?.Invoke(pageType, word);
    }
    
    public void GetDefinitionForView()
    {
        string word = DocumentVM.SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Dictionary", word);
        }
    }

    public void GetThesaurusForView()
    {
        string word = DocumentVM.SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Thesaurus", word);
        }
    }

    public void GetRhymeForView()
    {
        string word = DocumentVM.SelectedWord;
        if (!string.IsNullOrWhiteSpace(word))
        {
            OnRequestNavigation("Rhyming Dictionary", word);
        }
    }
    
    
}