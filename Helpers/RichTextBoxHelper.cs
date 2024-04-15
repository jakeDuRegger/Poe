using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Poe.Helpers;

// https://stackoverflow.com/questions/343468/richtextbox-wpf-binding
public static class RichTextBoxHelper
{
    public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
        "DocumentXaml",
        typeof(string),
        typeof(RichTextBoxHelper),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDocumentXamlChanged));

    public static string GetDocumentXaml(DependencyObject obj)
    {
        return (string)obj.GetValue(DocumentXamlProperty);
    }

    public static void SetDocumentXaml(DependencyObject obj, string value)
    {
        obj.SetValue(DocumentXamlProperty, value);
    }

    private static void OnDocumentXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        RichTextBox richTextBox = d as RichTextBox;
        if (richTextBox == null) return;

        string xaml = (string)e.NewValue;
        if (string.IsNullOrEmpty(xaml))
        {
            richTextBox.Document = new FlowDocument();
        }
        else
        {
            try
            {
                FlowDocument document = XamlReader.Parse(xaml) as FlowDocument;
                if (document != null)
                    richTextBox.Document = document;
            }
            catch (XamlParseException)
            {
                // Handle exceptions or ignore
            }
        }

        // Optional: Add a TextChanged handler to update the property when text changes
        TextChangedEventHandler textChangedEventHandler = null;
        textChangedEventHandler = new TextChangedEventHandler((sender, args) =>
        {
            FlowDocument doc = richTextBox.Document;
            TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml);
            SetDocumentXaml(richTextBox, Encoding.UTF8.GetString(stream.ToArray()));
            richTextBox.TextChanged -= textChangedEventHandler;
        });
        richTextBox.TextChanged += textChangedEventHandler;
    }
}
