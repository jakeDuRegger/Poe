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
                    // Todo we can come back to this to format it a bit better I really want to have a modern floating underline underneath the row headers
                    TitleTextBlock.Text = "Thesaurus";
                    (string synonyms, string antonyms) = await GetSynonyms(searchTerm);
                    if (!string.IsNullOrEmpty(synonyms) || !string.IsNullOrEmpty(antonyms))
                    {
                        
                        // Split synonyms and antonyms into lists
                        List<string> synonymList = synonyms.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> antonymList = antonyms.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        
                        // Create two tables each with one column
                        Table synonymTable = new Table();
                        Table antonymTable = new Table();
                        
                        // Calculate the number of columns needed for synonyms and antonyms
                        int synonymColumnsNeeded = (int)Math.Ceiling((double)synonymList.Count / 20);
                        int antonymColumnsNeeded = (int)Math.Ceiling((double)antonymList.Count / 20);

                        // Add columns to the synonym table
                        for (int i = 0; i < synonymColumnsNeeded; i++)
                        {
                            synonymTable.Columns.Add(new TableColumn());
                        }

                        // Add columns to the antonym table
                        for (int i = 0; i < antonymColumnsNeeded; i++)
                        {
                            antonymTable.Columns.Add(new TableColumn());
                        }
                        
                        // Create row groups for synonyms and antonyms
                        TableRowGroup synonymRowGroup = new TableRowGroup();
                        TableRowGroup antonymRowGroup = new TableRowGroup();
                        
                        // Create a row group for headers
                        TableRowGroup synonymHeaderRowGroup = new TableRowGroup();
                        TableRowGroup antonymHeaderRowGroup = new TableRowGroup();
                        
                        // Create the header row with custom styling
                        TableRow synonymHeaderRow = new TableRow();
                        synonymHeaderRow.FontSize = 16; // Set font size
                        synonymHeaderRow.FontWeight = FontWeights.Bold; // Set font weight

                        // Add the header to the header group
                        synonymHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Synonyms\n"))));
                        
                        // Add the header row to the header row group
                        synonymHeaderRowGroup.Rows.Add(synonymHeaderRow);
                        synonymTable.RowGroups.Add(synonymHeaderRowGroup);
                        
                        // Create the header row with custom styling
                        TableRow antonymHeaderRow = new TableRow();
                        antonymHeaderRow.FontSize = 16; // Set font size
                        antonymHeaderRow.FontWeight = FontWeights.Bold; // Set font weight
                        
                        antonymHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Antonyms\n"))));

                        // Add the header row to the header row group
                        antonymHeaderRowGroup.Rows.Add(antonymHeaderRow);
                        antonymTable.RowGroups.Add(antonymHeaderRowGroup);
                        
                       
                        // Add rows for synonyms
                        for (int i = 0; i < synonymList.Count; i++)
                        {
                            int rowIndex = i % 20;
                            int columnIndex = i / 20;

                            // Create a new row if necessary
                            if (rowIndex >= synonymRowGroup.Rows.Count)
                            {
                                synonymRowGroup.Rows.Add(new TableRow());
                            }

                            // Get the current row
                            TableRow currentRow = synonymRowGroup.Rows[rowIndex];

                            // Add a new cell to the current row
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(synonymList[i]))));
                        }

                        // Add rows for antonyms
                        for (int i = 0; i < antonymList.Count; i++)
                        {
                            int rowIndex = i % 20;
                            int columnIndex = i / 20;

                            // Create a new row if necessary
                            if (rowIndex >= antonymRowGroup.Rows.Count)
                            {
                                antonymRowGroup.Rows.Add(new TableRow());
                            }

                            // Get the current row
                            TableRow currentRow = antonymRowGroup.Rows[rowIndex];

                            // Add a new cell to the current row
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(antonymList[i]))));
                        }

                        // Add row groups to the tables
                        synonymTable.RowGroups.Add(synonymRowGroup);
                        antonymTable.RowGroups.Add(antonymRowGroup);

                        // Add the table to the flow document
                        SearchResultFlowDocument.Blocks.Add(synonymTable);
                        SearchResultFlowDocument.Blocks.Add(antonymTable);
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
                        // Split rhymes into chunks of 20 words
                        List<string> rhymeChunks = ChunkTextByWords(string.Join("\n", rhymes), 20);

                        // Create a table for rhymes
                        Table rhymeTable = new Table();

                        // Create a row group for headers
                        TableRowGroup headerRowGroup = new TableRowGroup();
                        
                        // Determine the number of columns needed (one column for every 20 rhymes)
                        int columnsNeeded = (int)Math.Ceiling(rhymeChunks.Count / 20.0);

                        // Add columns to the table
                        for (int i = 0; i < columnsNeeded; i++)
                        {
                            rhymeTable.Columns.Add(new TableColumn());
                        }
                        
                        // Create the header row with custom styling
                        TableRow headerRow = new TableRow();
                        headerRow.FontSize = 16; // Set font size
                        headerRow.FontWeight = FontWeights.Bold; // Set font weight

                        headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Rhymes\n"))));

                        // Add the header row to the header row group
                        headerRowGroup.Rows.Add(headerRow);
                        rhymeTable.RowGroups.Add(headerRowGroup);

                        // Create a row group for rhymes
                        TableRowGroup contentRowGroup = new TableRowGroup();
                        // Add cells for each rhyme, creating new rows and columns as needed
                        for (int i = 0; i < rhymes.Count; i++)
                        {
                            int rowIndex = i % 20;
                            int columnIndex = i / 20;

                            // Create a new row if necessary
                            if (rowIndex >= contentRowGroup.Rows.Count)
                            {
                                contentRowGroup.Rows.Add(new TableRow());
                            }

                            // Get the current row
                            TableRow currentRow = contentRowGroup.Rows[rowIndex];

                            // Add a new cell to the current row
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(rhymes[i]))));
                        }

                        // Add the content row group to the table
                        rhymeTable.RowGroups.Add(contentRowGroup);

                        // Add the table to the flow document
                        SearchResultFlowDocument.Blocks.Add(rhymeTable);
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


    // Function to split text into chunks of a specified number of words
    private List<string> ChunkTextByWords(string text, int wordsPerChunk)
    {
        List<string> chunks = new List<string>();
        string[] words = text.Split(' ');
        StringBuilder chunk = new StringBuilder();
        int wordsInChunk = 0;

        foreach (string word in words)
        {
            if (wordsInChunk >= wordsPerChunk)
            {
                chunks.Add(chunk.ToString().TrimEnd());
                chunk.Clear();
                wordsInChunk = 0;
            }

            chunk.Append(word + " ");
            wordsInChunk++;
        }

        if (chunk.Length > 0)
        {
            chunks.Add(chunk.ToString().TrimEnd());
        }

        return chunks;
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