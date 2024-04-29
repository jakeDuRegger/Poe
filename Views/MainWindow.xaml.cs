using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Material.Icons;
using Material.Icons.WPF;
using Poe.ViewModels;
using Poe.ViewModels.Factory;
using Poe.Views;


namespace Poe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = ViewModelFactory.CreateMainWindowVM();

        // Set the DataContext for data binding
        DataContext = viewModel;

        // Subscribe to the navigation request event
        viewModel.RequestNavigation += HandleNavigationRequest;

        // So I guess this sets the minimum height of MainRtb whenever the user updates it???
        MainRtb.LayoutUpdated += MainRtb_LayoutUpdated;
    }

    private void MainRtb_LayoutUpdated(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.RtbHeight = MainRtb.GetDocumentHeight();
        }
    }

    // Ctrl + Shift + V functionality in MainRtb todo fix this
    private void RichTextBox_Paste(object sender, ExecutedRoutedEventArgs e)
    {
        if (!Clipboard.ContainsText()) return;
        MainRtb.Selection.Text = Clipboard.GetText(TextDataFormat.Text);
        e.Handled = true;
    }

    // Custom context menu in MainRtb todo move this to view model.
    private void MainRtb_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        int indexCounter = 0;
        // Clear any previous spelling suggestions

        // Remove existing spelling suggestions and "Ignore All" option
        RemoveExistingItems();

        // Get the position of the mouse
        Point mousePosition = Mouse.GetPosition(MainRtb);

        // Get the position in the text
        TextPointer? position = MainRtb.GetPositionFromPoint(mousePosition, true);


        if (DataContext is MainWindowViewModel viewModel)
        {
            //  Only try to append word if there's a singular word
            if (!viewModel.MultipleWords(MainRtb.Selection.Text))
            {
                // Get the word at the caret or the selected word
                string? word = GetWordAtPosition(MainRtb) ?? string.Empty; // Ensure word is not null
                string selectedText = MainRtb.Selection.Text;
                if (!string.IsNullOrEmpty(selectedText.Trim()))
                    viewModel.SetCurrentSelectedWord(selectedText);
                if (!string.IsNullOrEmpty(word))
                    viewModel.SelectedWord = word; // Correctly set the selected word
            }
        }

        if (position != null)
        {
            SpellingError? error = MainRtb.GetSpellingError(position);
            if (error != null)
            {
                // Add suggestions to the context menu
                foreach (string suggestion in error.Suggestions)
                {
                    MenuItem menuItem = new MenuItem
                    {
                        Header = suggestion,
                        FontWeight = FontWeights.Bold,
                        Command = EditingCommands.CorrectSpellingError,
                        CommandParameter = suggestion,
                        CommandTarget = MainRtb
                    };
                    menuItem.Icon = new MaterialIcon()
                    {
                        Kind = MaterialIconKind.Spellcheck
                    };
                    if (MainRtb.ContextMenu != null)
                        MainRtb.ContextMenu.Items.Insert(0, menuItem); // Insert at the beginning

                    indexCounter++;
                }

                // Add a menu item gray and height of one.
                if (MainRtb.ContextMenu != null)
                {
                    MainRtb.ContextMenu.Items.Insert(indexCounter, new MenuItem
                    {
                        Height = 1,
                        Name = "separator",
                        Background = new SolidColorBrush(Colors.LightGray)
                    });
                    MenuItem ignoreAllItem = new MenuItem
                    {
                        Header = "Ignore All",
                        Command = EditingCommands.IgnoreSpellingError,
                        CommandTarget = MainRtb
                    };
                    ignoreAllItem.Icon = new MaterialIcon()
                    {
                        Kind = MaterialIconKind.NotificationsNone
                    };
                    // Add the Ignore All option
                    MainRtb.ContextMenu.Items.Insert(indexCounter + 1, ignoreAllItem);
                }
            }
        }
    }


    // https://stackoverflow.com/questions/3934422/wpf-richtextbox-get-whole-word-at-current-caret-position
    /**
     * I have no idea how this works. F### me.
     */
    private string? GetWordAtPosition(RichTextBox richTextBox)
    {
        TextPointer caretPosition = richTextBox.CaretPosition;

        // Check if the caret is at the start of the document
        if (caretPosition.CompareTo(richTextBox.Document.ContentStart) == 0)
        {
            // If the caret is at the start, there is no previous word or whitespace
            return "";
        }

        // Move the caret to the beginning of the word
        while (caretPosition != null &&
               (caretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text ||
                caretPosition.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
        {
            string textInRun = caretPosition.GetTextInRun(LogicalDirection.Backward);
            if (!string.IsNullOrEmpty(textInRun))
            {
                // Scan the text run for the first whitespace character from the end
                for (int i = textInRun.Length - 1; i >= 0; i--)
                {
                    if (char.IsWhiteSpace(textInRun[i]))
                    {
                        // Move the pointer to right after the whitespace
                        caretPosition =
                            caretPosition.GetPositionAtOffset(-(textInRun.Length - i - 1), LogicalDirection.Backward);
                        goto FoundStart;
                    }
                }
            }

            // Move to the next text run if no whitespace found in the current run
            TextPointer nextPosition = caretPosition.GetNextContextPosition(LogicalDirection.Backward);
            if (nextPosition != null)
                caretPosition = nextPosition;
            else
                break; // Exit if no further positions are available (at the start of the document)
        }

        FoundStart:

        TextPointer start = caretPosition;


        // Move the caret to the end of the word
        while (caretPosition != null &&
               (caretPosition.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text ||
                caretPosition.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None))
        {
            string textInRun = caretPosition.GetTextInRun(LogicalDirection.Forward);
            if (!string.IsNullOrEmpty(textInRun) && char.IsWhiteSpace(textInRun[0]))
                break;
            caretPosition = caretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
        }

        TextPointer? end = caretPosition;

        return new TextRange(start, end).Text;
    }


    /**
     * Removes the existing items from context menu list of the document.
     */
    private void RemoveExistingItems()
    {
        if (MainRtb.ContextMenu != null)
            for (int i = MainRtb.ContextMenu.Items.Count - 1; i >= 0; i--)
            {
                if (MainRtb.ContextMenu.Items[i] is MenuItem menuItem &&
                    (menuItem.Command == EditingCommands.CorrectSpellingError ||
                     menuItem.Command == EditingCommands.IgnoreSpellingError || menuItem.Name == "separator"))
                {
                    MainRtb.ContextMenu.Items.RemoveAt(i);
                }
            }
    }

    // Tool Bar load in. todo figure out and document why we need this
    private void ToolBar_Loaded(object sender, RoutedEventArgs e)
    {
        ToolBar toolBar = sender as ToolBar;
        var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
        if (overflowGrid != null)
        {
            overflowGrid.Visibility = Visibility.Collapsed;
        }
    }

    // All of these below can stay here in xaml.cs
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeRestoreWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void DragWindowResize(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            MaximizeRestoreWindow(sender, null);
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    // todo Add spellchecking turn on / off settings


    /*
     *  Navigation to look up a word
     */

    private void HandleNavigationRequest(string pageType, string word)
    {
        // Navigate using your frame
        SearchResultsFrame.Navigate(new SearchResultsPage(pageType, word));
    }

    private void LookupDefinition_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            // Navigate to the SearchResultsPage with "definition" as the search type
            HandleNavigationRequest("Dictionary", selectedText);
        }
        else if (DataContext is MainWindowViewModel viewModel)
        {
            // Use the word at the caret position
            viewModel.GetDefinitionForView();
        }
    }

    private void LookupSynonyms_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            HandleNavigationRequest("Thesaurus", selectedText);
        }
        else if (DataContext is MainWindowViewModel viewModel)
        {
            // Use the word at the caret position
            viewModel.GetThesaurusForView();
        }
    }

    private void LookupRhymes_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            HandleNavigationRequest("Rhyming Dictionary", selectedText);
        }
        else if (DataContext is MainWindowViewModel viewModel)
        {
            // Use the word at the caret position
            viewModel.GetRhymeForView();
        }
    }

    // Content box font selection!
    private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainRtb != null && FontFamilyComboBox.SelectedItem is FontFamily fontFamily)
        {
            MainRtb.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
        }
    }

    private void ZoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ZoomComboBox.SelectedItem is ComboBoxItem selectedZoom)
        {
            string zoomText = selectedZoom.Content.ToString().Replace("%", "");
            if (double.TryParse(zoomText, out double zoomValue))
            {
                var scale = MainRtb.LayoutTransform as ScaleTransform;
                double factor = zoomValue / 100.0;
                scale.ScaleX = factor;
                scale.ScaleY = factor;
            }
        }
    }

    // todo Content box for bullets / numbering


    //Todo File Dialog options / Create new file / Open pre existing file / Templating?? 
    private void FileSettingsClick(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        button.ContextMenu.IsEnabled = true;
        button.ContextMenu.PlacementTarget = button;
        button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        button.ContextMenu.IsOpen = true;
    }


    private async void AnalyzeRhymes_Click(object sender, RoutedEventArgs e)
    {
        Dictionary<char, List<string>> rhymeScheme = new Dictionary<char, List<string>>();
        if (DataContext is MainWindowViewModel viewModel)
        {
            var text = MainRtb.Selection.Text;
            
            // Get the lines from the selected text
            var lines = await viewModel.GetLinesFromText(text);
            
            // Get the end words from lines.
            var endWords = await viewModel.GetEndWordsFromText(lines);

            rhymeScheme = await viewModel.GetRhymeSchemeForView(endWords);
            
            

            // Use StringBuilder to create the display format
            var rhymeSchemeDisplay = new StringBuilder();

            // Append each entry in the rhymeScheme dictionary to the StringBuilder
            foreach (var entry in rhymeScheme)
            {
                // Add each key and its corresponding values formatted as 'Key: Value, Value'
                rhymeSchemeDisplay.AppendLine($"{entry.Key}: {string.Join(", ", entry.Value)}");
            }

            // Display the results in a MessageBox
            MessageBox.Show(
                $"There were a total of: {rhymeScheme.Count} rhyme schemes in your text.\n\n" +
                rhymeSchemeDisplay.ToString(),
                "Rhyme Schemes Analysis",  // Title for the MessageBox
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("ViewModel is not available.");
        }
    }

    private void MainRtb_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // We want to only call the event once there are a sufficient amount of lines
        // We need to cache the results of our already identified rhyme schemes if the end words are still the same.
        // We need to update the cache if the end words change.
        // We can do this with a simple json format of the respective A: "", B: "", etc.
        // We can divide the text into two sections to run in parallel.
        // One function is simply a simple checker that checks if anything has changed since its last historical version
        // The second function starts from the last TextPointer and checks if textChanged from that point on.
        // I wonder if this requires an additional custom event handler that checks position
        // Adding a debounce timer would help to reduce resource overhead and not annoy the user.
        // Debounce timer + only being called on worthy changes to pre-existing text would be the goal I assume.

        /*// Access the text from the RichTextBox
        TextRange textRange = new TextRange(MainRtb.Document.ContentStart, MainRtb.Document.ContentEnd);

        string textContent = textRange.Text;

        // Normalize newline characters and split the text into lines
        var lines = textContent.Replace("\r\n", "\n").Split('\n');
        // Check if there are at least two non-empty lines
        if (lines.Count(line => !string.IsNullOrWhiteSpace(line)) >= 3)
        {
            MessageBox.Show("There are two lines!");
        }*/
    }

    private void BreakPage_Click(object sender, RoutedEventArgs e)
    {
        MainRtb?.BreakPage();
    }

    private async void TestEndWords(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            string text = MainRtb.Selection.Text;
            var endWords = await viewModel.GetLinesFromText(text);
            string result = null;
            foreach (var word in endWords)
            {
                result += word + "\n";
            }
            MessageBox.Show(result);
        }
    }
}