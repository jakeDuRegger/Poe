using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Poe.Models.Document;

public class MainRichTextBox : RichTextBox
{
    private PageBreakAdorner? _pageBreakAdorner;

    private double DocumentHeight { get; set; } 
    private int PageNumber { get; set; }


    public MainRichTextBox()
    {
        DocumentHeight= ConvertInches(8.5); // Default of 8.5inches
                                            // todo add the 'default' document height here passed in as a 'setting'.
        PageNumber = 1; // Init with one page.
                        // todo add the pageNumber load in with "setting" / "cache".
    }

    private void AddPageBreak(double newBreakPosition)
    {
        if (_pageBreakAdorner == null)
        {
            _pageBreakAdorner = new PageBreakAdorner(this, DocumentHeight);
            AdornerLayer? layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null) layer.Add(_pageBreakAdorner);
        }
        else
        {
            _pageBreakAdorner.AddNewRectangle(newBreakPosition);
        }
        PageNumber++;
    }
    
    public void BreakPage()
    {
        double newBreakPosition = DocumentHeight;

        CaretPosition.InsertTextInRun("\f"); // End the document for printing

        if (PageNumber > 1)
        {
            newBreakPosition = PageNumber * DocumentHeight;
        }

        AddPageBreak(newBreakPosition);
    }

    private bool ShouldBreak()
    {
        // Check if caret position is at end of the document length
        var nextInsertion = CaretPosition.GetCharacterRect(LogicalDirection.Forward);
        return nextInsertion.Bottom > DocumentHeight;
    }

    private void RestrictCaret(PageBreakAdorner pageBreakAdorner)
    {
        // // Remove the ability for the caret to be within or around the adorner.
        // var caretRect = CaretPosition.GetCharacterRect(LogicalDirection.Forward);
        // if (caretRect.Bottom > pageBreakAdorner.PageBreakHeight)
        // {
        //     // Like if the user enters a new line or wraps this should account, yeah?
        //     // It should either send them up or back down
        //     // If they're adding content it needs to send them down. 
        //     // If they're deleting content it needs to send them up.
        // }
    }
    

    private static double ConvertInches(double inches)
    {
        // pixel = 1/96 of inch
        // 96 * inches = pixel amount;
        return inches * 96;
    }
}