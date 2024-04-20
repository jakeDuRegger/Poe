using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Poe.API;
using Poe.Models.API;

namespace Poe.ViewModels;

public partial class SearchResultsViewModel : ObservableObject
{
    private readonly DataMuse _dataMuse = new DataMuse();

    [ObservableProperty]
    private Visibility _pageBorderVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private FlowDocument _searchResultFlowDocument = new();

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _searchTerm;

    public ICommand SearchCommand { get; private set; }

    public SearchResultsViewModel(string searchType, string searchTerm)
    {
        async void Execute() => await ExecuteSearch();
        SearchCommand = new RelayCommand(Execute);
        InitializeSearchResultFlowDocument();
        InitializeAsync(searchType, searchTerm);
    }

    private async void InitializeAsync(string searchType, string searchTerm)
    {
        await LoadResults(searchType, searchTerm);
    }


    

    // Method to initialize FlowDocument with default settings
    private void InitializeSearchResultFlowDocument()
    {
        SearchResultFlowDocument.ColumnWidth = 200;
        SearchResultFlowDocument.ColumnGap = 20;
        SearchResultFlowDocument.ColumnRuleWidth = 1;
        SearchResultFlowDocument.ColumnRuleBrush = new SolidColorBrush(Colors.Gray);
        SearchResultFlowDocument.TextAlignment = TextAlignment.Justify;
        SearchResultFlowDocument.IsColumnWidthFlexible = true;
    }

    // Async method to load results
    private async Task LoadResults(string searchType,string searchTerm)
    {
        // Make sure its visible.
        PageBorderVisibility = Visibility.Visible;

        // Clear existing content
        SearchResultFlowDocument.Blocks.Clear();
        
        switch (searchType)
        {
            case "Dictionary":
                Title = "Dictionary";
                List<string> definitions = await GetDefinitionForPage(searchTerm);
                if (definitions.Count > 0)
                {
                    if (definitions.Count == 1)
                    {
                        // Create header as singular if only one definition
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Definition for '{searchTerm}'"))
                        {
                            FontWeight = FontWeights.Bold
                        });
                    }
                    else
                    {
                        // Create header as plural if multiple definitions are found
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Definitions for '{searchTerm}'"))
                        {
                            FontWeight = FontWeights.Bold
                        });
                    }

                    // Add separator for header
                    SearchResultFlowDocument.Blocks.Add(new BlockUIContainer(new Separator()));
                    // Add space.
                    SearchResultFlowDocument.Blocks.Add(new Paragraph());

                    foreach (string definition in definitions)
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run(definition)));
                        // Add a separator after the definition
                        Separator separator = new Separator();
                        separator.MaxWidth = SearchResultFlowDocument.Parent is FrameworkElement parent ? parent.ActualWidth * 0.75 : double.NaN;
                        SearchResultFlowDocument.Blocks.Add(new BlockUIContainer(separator));
                    }
                }
                else
                {
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No definitions found.")));
                }

                break;

            case "Thesaurus":
                Title = "Thesaurus";
                var (synonyms, antonyms) = await GetThesaurus(searchTerm);
                if (synonyms.Count > 0 || antonyms.Count > 0)
                {
                    // Add a header for synonyms
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Synonyms for '{searchTerm}'"))
                    {
                        FontWeight = FontWeights.Bold
                    });

                    // Add separator for header
                    SearchResultFlowDocument.Blocks.Add(new BlockUIContainer(new Separator()));

                    // Create a paragraph to hold the synonyms
                    Paragraph synonymsParagraph = new Paragraph();
                    // Create 'rows' depending on size of list
                    int rows = 0;
                    if (synonyms.Count % 2 == 0)
                    {
                        rows = synonyms.Count / 5;
                    }
                    else
                    {
                        rows = synonyms.Count / 5 + 1;
                    }

                    // Determine the number of items per column
                    int itemsPerRow = (int)Math.Ceiling((double)synonyms.Count / rows);

                    int counter = 0;
                    foreach (var synonym in synonyms)
                    {
                        // Add each synonym to the paragraph with a fixed width Run
                        Run synonymRun = new Run(synonym)
                        {
                            FontSize = 14
                        };
                        synonymsParagraph.Inlines.Add(synonymRun);
                        synonymsParagraph.Inlines.Add(new Run("   ")); // Add a space between words

                        counter++;

                        // Add a line break to start a new 'row' after a set number of items
                        if (counter % itemsPerRow == 0 && counter < synonyms.Count)
                        {
                            synonymsParagraph.Inlines.Add(new LineBreak());
                            synonymsParagraph.Inlines.Add(new LineBreak());
                        }
                    }

                    SearchResultFlowDocument.Blocks.Add(synonymsParagraph);


                    // Add a header for antonyms
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Antonyms for '{searchTerm}'"))
                    {
                        FontWeight = FontWeights.Bold
                    });

                    // Add separator for header
                    SearchResultFlowDocument.Blocks.Add(new BlockUIContainer(new Separator()));

                    // Create a paragraph to hold the antonyms
                    Paragraph antonymsParagraph = new Paragraph();
                    // Create 'rows' depending on size of list
                    int antRows = 0;
                    if (synonyms.Count % 2 == 0)
                    {
                        antRows = synonyms.Count / 5;
                    }
                    else
                    {
                        antRows = synonyms.Count / 5 + 1;
                    }

                    // Determine the number of items per column
                    int itemsPerRowInAntonyms = (int)Math.Ceiling((double)antonyms.Count / antRows);

                    int antCounter = 0;
                    foreach (var antonym in antonyms)
                    {
                        // Add each synonym to the paragraph with a fixed width Run
                        Run antonymRun = new Run(antonym)
                        {
                            FontSize = 14
                        };
                        antonymsParagraph.Inlines.Add(antonymRun);
                        antonymsParagraph.Inlines.Add(new Run("   ")); // Add a space between words

                        antCounter++;

                        // Add a line break to start a new 'row' after a set number of items
                        if (antCounter % itemsPerRowInAntonyms == 0 && antCounter < antonyms.Count)
                        {
                            antonymsParagraph.Inlines.Add(new LineBreak());
                            antonymsParagraph.Inlines.Add(new LineBreak());
                        }
                    }

                    SearchResultFlowDocument.Blocks.Add(antonymsParagraph);
                }
                else
                {
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No synonyms or antonyms found.")));
                }

                break;


            case "Rhyming Dictionary":
                Title = "Rhyming Dictionary";
                List<string> rhymes = await GetRhymes(searchTerm);
                if (rhymes.Count > 0)
                {
                    // Create header
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Rhymes for '{searchTerm}'"))
                    {
                        FontWeight = FontWeights.Bold
                    });
                    // Add separator for header
                    SearchResultFlowDocument.Blocks.Add(new BlockUIContainer(new Separator()));

                    foreach (string rhyme in rhymes)
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run(rhyme)));
                    }
                }
                else
                {
                    SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No rhymes found.")));
                }

                break;

            default:
                Title = "Poe";
                SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("Invalid search type.")));
                break;
        }
    }

    private async Task ExecuteSearch()
    {
        var searchTerm = SearchTerm?.Trim();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            // The Title property should be set based on user interaction in the view that changes the search type
            await LoadResults(Title, searchTerm);
        }
    }

// Calls the back-end function to look up definitions for a given word
    private async Task<List<string>> GetDefinitionForPage(string word)
    {
        word = word.ToLower();
        // return await _mwApi.GetDictionaryDefinition(word);
        return await _dataMuse.GetDefinition(word);
    }

// Calls the back-end function to look up synonyms for a given word
    private async Task<(List<string> Synonyms, List<string> Antonyms)> GetThesaurus(string word)
    {
        word = word.ToLower();
        var synonyms = await _dataMuse.GetSynonym(word);
        var antonyms = await _dataMuse.GetAntonyms(word);

        return (synonyms, antonyms);
    }


// Calls the back-end function to look up rhymes for a given word
    private async Task<List<string>> GetRhymes(string word)
    {
        word = word.ToLower();
        return await _dataMuse.GetRhymes(word);
    }
}