using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Poe.API;

namespace Poe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MerriamWebsterAPI _mwApi = new MerriamWebsterAPI();

    public MainWindow()
    {
        InitializeComponent();
        /*if (MessageBox.Show("Are you sure you want to weave!!!?????",
                "Save file",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            // Do something here
        }*/
    }
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
    
    
    // Todo add the Meriam Webster API functionality
    // Context menu selection (Lookup synonyms) and (Lookup definition).
    private async void LookupDefinition(string word)
    {
        // Make sure lookup is case insensitive
        word = word.ToLower();
        var definitions = await _mwApi.GetDictionaryDefinition(word);

        if (definitions.Length <= 0)
        {
            MessageBox.Show("Error occured");
        }
        
        MessageBox.Show(definitions);
    }


    private async void LookupSynonyms(string word)
    {
        // Make sure lookup is case insensitive
        word = word.ToLower();

        var (synonyms, antonyms) = await _mwApi.GetThesaurus(word);

        if (string.IsNullOrEmpty(synonyms) && string.IsNullOrEmpty(antonyms))
        {
            MessageBox.Show("Error occurred");
        }
        else
        {
            MessageBox.Show($"Synonyms:\n{synonyms}\nAntonyms:\n{antonyms}", "Synonyms and Antonyms");
        }
    }
    // Event handling for context menu selection
    private void LookupDefinition_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            LookupDefinition(selectedText);
        }
    }

    private void LookupSynonyms_Click(object sender, RoutedEventArgs e)
    {
        string selectedText = MainRtb.Selection.Text;
        if (!string.IsNullOrWhiteSpace(selectedText))
        {
            LookupSynonyms(selectedText);
        }
    }
    
    //Todo Create pagination for MainRtb
    
    //Todo File Dialog options / Create new file / Open pre existing file / Templating?? -- Maybe use poemDB to help poets?
    
    //Todo Rhyme scheme
    
    //Todo Fix basic context menu not appearing (cut, copy, paste, etc.)
    

}