using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Poe.API;

namespace Poe.Views;

public partial class SearchResultsPage : Page
{
    // Maybe make generic type for these APIs bc they might get out of hand
    private readonly MerriamWebsterApi _mwApi = new MerriamWebsterApi();
    private readonly DataMuse _dataMuse = new DataMuse();
    
    
    public SearchResultsPage(string searchType, string searchTerm)
    {
        InitializeComponent();
        SearchResultFlowDocument = new FlowDocument
        {
            ColumnWidth = 200,
            ColumnGap = 20,
            ColumnRuleWidth = 1,
            ColumnRuleBrush = new SolidColorBrush(Colors.Gray),
            TextAlignment = TextAlignment.Justify,
            IsColumnWidthFlexible = true
        };
        
        // Set the DataContext for bindings if you are using MVVM pattern
        DataContext = this;

        // Start loading results asynchronously
        LoadResults(searchType, searchTerm);
    }

    private async void LoadResults(string searchType, string searchTerm)
    {
        // Ensure UI updates happen on the main thread
        await Dispatcher.InvokeAsync(async () =>
        {
            // Make sure its visible.
            PageGrid.Visibility = Visibility.Visible;

            // Clear existing content
            SearchResultFlowDocument.Blocks.Clear();

            switch (searchType)
            {
                case "definition":
                    TitleTextBlock.Text = "Dictionary";
                    string definitions = await GetDefinition(searchTerm);
                    if (!string.IsNullOrEmpty(definitions))
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run(definitions)));
                    }
                    else
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No definitions found.")));
                    }

                    break;

                case "synonym":
                    TitleTextBlock.Text = "Thesaurus";
                    (string synonyms, string antonyms) = await GetSynonyms(searchTerm);
                    if (!string.IsNullOrEmpty(synonyms) || !string.IsNullOrEmpty(antonyms))
                    {
                        SearchResultFlowDocument.Blocks.Add(
                            new Paragraph(new Run($"Synonyms:\n{synonyms}\n\nAntonyms:\n{antonyms}")));
                    }
                    else
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No synonyms or antonyms found.")));
                    }

                    break;

                case "rhyme":
                    TitleTextBlock.Text = "Rhyming Dictionary";
                    // todo add title to title bar 
                    // title = Rhyming Dictionary
                    List<string> rhymes = await GetRhymes(searchTerm);
                    if (rhymes.Count > 0)
                    {
                        StringBuilder rhymeList = new StringBuilder();
                        foreach (var rhyme in rhymes)
                        {
                            rhymeList.AppendLine(rhyme);
                        }

                        SearchResultFlowDocument.Blocks.Add(
                            new Paragraph(new Run($"Rhymes for '{searchTerm}':\n{rhymeList}")));
                    }
                    else
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No rhymes found.")));
                    }

                    break;

                default:
                    TitleTextBlock.Text = "Poe";
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("Invalid search type.")));
                    break;
            }

            // Since we are updating the FlowDocument, which is a UI element, make sure this line is within the Dispatcher.InvokeAsync
            PageFlowDocumentScrollViewer.Document = SearchResultFlowDocument;
        });
    }
    
    private FlowDocument SearchResultFlowDocument { get; set; }


    
    private void ClosePage(object sender, RoutedEventArgs e)
    {
        // Switch visibility between Grid1 and Grid2
        if (PageGrid.Visibility == Visibility.Visible)
        {
            PageGrid.Visibility = Visibility.Collapsed;
        }
    }


    
    
    // Calls the back-end function to look up definitions for a given word
    private async Task<string> GetDefinition(string word)
    {
        word = word.ToLower();
        return await _mwApi.GetDictionaryDefinition(word);
    }

    // Calls the back-end function to look up synonyms for a given word
    private async Task<(string, string)> GetSynonyms(string word)
    {
        word = word.ToLower();
        return await _mwApi.GetThesaurus(word);
    }

    // Calls the back-end function to look up rhymes for a given word
    private async Task<List<string>> GetRhymes(string word)
    {
        word = word.ToLower();
        return await _dataMuse.GetRhymes(word);
    }
    
}