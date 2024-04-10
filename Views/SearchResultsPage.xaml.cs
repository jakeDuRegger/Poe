using System.DirectoryServices;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Poe.API;
using Poe.ViewModels;

namespace Poe.Views;

public partial class SearchResultsPage : Page
{
    public SearchResultsPage(string searchType, string searchTerm)
    {
        InitializeComponent();
        DataContext = new SearchResultsViewModel(searchType, searchTerm);
    }


    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // We trigger the search command from the ViewModel when Enter is pressed
            // This requires the command to be accessible as a public property of the ViewModel
            var viewModel = DataContext as SearchResultsViewModel;
            viewModel?.SearchCommand.Execute(null);
        }
    }
    
    private void ClosePage(object sender, RoutedEventArgs e)
    {
        // Collapses the page
        if (PageGrid.Visibility == Visibility.Visible)
        {
            PageGrid.Visibility = Visibility.Collapsed;
        }
    }
}