using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Material.Icons;
using Material.Icons.WPF;
using Poe.API;
using Poe.Views;
using SpellCheck;
using SpellCheck.Dictionaries;


namespace Poe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
    }
    // Ctrl + Shift + V functionality in MainRtb todo fix this
    private void RichTextBox_Paste(object sender, ExecutedRoutedEventArgs e)
    {
        if (!Clipboard.ContainsText()) return;
        MainRtb.Selection.Text = Clipboard.GetText(TextDataFormat.Text);
        e.Handled = true;
    }

    // Custom context menu in MainRtb
    private void MainRtb_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        int indexCounter = 0;
        // Clear any previous spelling suggestions
        try
        {
            // Remove existing spelling suggestions and "Ignore All" option
            RemoveExistingItems();
            
            // Get the position of the mouse
            Point mousePosition = Mouse.GetPosition(MainRtb);

            // Get the position in the text
            TextPointer position = MainRtb.GetPositionFromPoint(mousePosition, true);

            // Use the helper function to select the word
            SelectWordAtPosition(position);
            
            if (position != null)
            {
                SpellingError error = MainRtb.GetSpellingError(position);
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
                        MainRtb.ContextMenu.Items.Insert(0, menuItem); // Insert at the beginning
                        
                        indexCounter++;
                    }
                    
                    // Add a menu item gray and height of one.
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
                    MainRtb.ContextMenu.Items.Insert(indexCounter + 1,ignoreAllItem);
                }
            }
        }
        catch
        {
            // ignored
        }
    }
    
    private void SelectWordAtPosition(TextPointer position)
    {
        if (position == null)
        {
            return;
        }

        // Find the start of the word
        TextPointer start = position;
        while (start != null && !char.IsWhiteSpace((start.GetTextInRun(LogicalDirection.Backward)).FirstOrDefault()))
        {
            start = start.GetNextInsertionPosition(LogicalDirection.Backward);
            if (start == null || start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
            {
                break;
            }
        }

        // Find the end of the word
        TextPointer end = position;
        while (end != null && !char.IsWhiteSpace((end.GetTextInRun(LogicalDirection.Forward)).FirstOrDefault()))
        {
            end = end.GetNextInsertionPosition(LogicalDirection.Forward);
            if (end == null || end.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None)
            {
                break;
            }
        }

        if (start != null && end != null)
        {
            MainRtb.Selection.Select(start, end);
        }
    }

    
    private void RemoveExistingItems()
    {
        for (int i = MainRtb.ContextMenu.Items.Count - 1; i >= 0; i--)
        {
            if (MainRtb.ContextMenu.Items[i] is MenuItem menuItem && (menuItem.Command == EditingCommands.CorrectSpellingError || menuItem.Command == EditingCommands.IgnoreSpellingError || menuItem.Name== "separator"))
            {
                MainRtb.ContextMenu.Items.RemoveAt(i);
            }
        }
    }
    // Tool Bar load in.

    private void ToolBar_Loaded(object sender, RoutedEventArgs e)
    {
        ToolBar toolBar = sender as ToolBar;
        var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
        if (overflowGrid != null)
        {
            overflowGrid.Visibility = Visibility.Collapsed;
        }
    }

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
    
    // Create pop up for showing definitions and synonyms
    // private void ShowPopup(string message)
    // {
    //     PopupText.Text = message;
    //     InfoPopup.IsOpen = true;
    // }
    
    

    private void LookupDefinition_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            // Navigate to the SearchResultsPage with "definition" as the search type
            SearchResultsFrame.Navigate(new SearchResultsPage("definition", selectedText));
        }
    }
    private void LookupSynonyms_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            // Navigate to the SynonymsPage and pass the selected text
            SearchResultsFrame.Navigate(new SearchResultsPage("synonym", selectedText));
        }
    }
    private void LookupRhymes_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            // Navigate to the RhymesPage and pass the selected text
            SearchResultsFrame.Navigate(new SearchResultsPage("rhyme", selectedText));
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
    // todo Content box for bullets / numbering


    
    
    
}

//Todo Create pagination for MainRtb
    
    //Todo File Dialog options / Create new file / Open pre existing file / Templating?? -- Maybe use poemDB to help poets?
    
    //Todo Rhyme scheme



