using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
                    List<string> definitions = await GetDefinition(searchTerm);
                    if (definitions.Count > 0)
                    {
                        foreach  (string definition in definitions) {
                            SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run(definition)));

                        }
                    }
                    else
                    {
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("No definitions found.")));
                    }

                    break;

                case "synonym":
                    TitleTextBlock.Text = "Thesaurus";
                    var (synonyms, antonyms) = await GetThesaurus(searchTerm);
                    if (synonyms.Count > 0 || antonyms.Count > 0)
                    {
                        // Add a header for synonyms
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("Synonyms"))
                        {
                            FontWeight = FontWeights.Bold
                        });
                        
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
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run("Antonyms"))
                        {
                            FontWeight = FontWeights.Bold
                        });
                        
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


                case "rhyme":
                    TitleTextBlock.Text = "Rhyming Dictionary";
                    List<string> rhymes = await GetRhymes(searchTerm);
                    if (rhymes.Count > 0)
                    {
                        // Add a header for rhymes
                        SearchResultFlowDocument.Blocks.Add(new Paragraph(new Run($"Rhymes for '{searchTerm}':"))
                        {
                            FontWeight = FontWeights.Bold
                        });

                        // Create and configure the ListView for rhymes
                        ListView rhymeListView = new ListView
                        {
                            ItemsSource = rhymes,
                            Margin = new Thickness(5)
                        };

                        // Set the ItemsPanel to be a WrapPanel
                        rhymeListView.ItemsPanel =
                            new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));

                        // Add the ListView to the FlowDocument
                        BlockUIContainer rhymeContainer = new BlockUIContainer(rhymeListView);
                        SearchResultFlowDocument.Blocks.Add(rhymeContainer);
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
            // Search Bar Feature


            // Since we are updating the FlowDocument, which is a UI element, make sure this line is within the Dispatcher.InvokeAsync
            PageFlowDocumentScrollViewer.Document = SearchResultFlowDocument;
        });
    }


    private void UpdateListViewItemsSource(ListView listView, List<string> items, int maxItems)
    {
        // Limit the number of items displayed
        var limitedItems = items.Take(maxItems).ToList();
        listView.ItemsSource = limitedItems;

        // Add a "Show More" button if there are more items to display
        if (items.Count > maxItems)
        {
            Button showMoreButton = new Button
            {
                Content = "Show More",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            showMoreButton.Click += (sender, e) =>
            {
                // Update the ItemsSource with all items
                listView.ItemsSource = items;
                // Remove the "Show More" button
                var container = listView.Parent as BlockUIContainer;
                if (container != null)
                {
                    SearchResultFlowDocument.Blocks.Remove(container);
                    BlockUIContainer newContainer = new BlockUIContainer(listView);
                    SearchResultFlowDocument.Blocks.Add(newContainer);
                }
            };

            var container = new BlockUIContainer(showMoreButton);
            SearchResultFlowDocument.Blocks.Add(container);
        }
    }
    


    private FlowDocument SearchResultFlowDocument { get; set; }


    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        string searchTerm = SearchTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            // Perform search based on the current search type (e.g., definition, synonym, rhyme)
            string currentSearchType = TitleTextBlock.Text.ToLower();
            switch (currentSearchType)
            {
                case "dictionary":
                    LoadResults("definition", searchTerm);
                    break;
                case "thesaurus":
                    LoadResults("synonym", searchTerm);
                    break;
                case "rhyming dictionary":
                    LoadResults("rhyme", searchTerm);
                    break;
                default:
                    MessageBox.Show("Please select a valid search type.");
                    break;
            }
        }
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchButton_Click(sender, e);
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


    // Calls the back-end function to look up definitions for a given word
    private async Task<List<string>> GetDefinition(string word)
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