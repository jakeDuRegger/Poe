using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Poe.Models.Document;

public class PageBreakAdorner : Adorner
{
    private double _pageBreakHeight;

    private List<double> _rectanglePositions;
    
    public PageBreakAdorner(UIElement adornedElement, double documentHeight) : base(adornedElement)
    {
        _pageBreakHeight = documentHeight + 10;
        IsHitTestVisible = false; // Makes sure adorner doesn't interfere with text editing
        _rectanglePositions = new List<double> { _pageBreakHeight }; // Add the original at page break height.
    }
    
    private void Update()
    {
        InvalidateVisual(); // Updates the UI
    }
    
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        
        SolidColorBrush fillBrush  = new SolidColorBrush(Color.FromRgb(50, 50, 48)); // Equivalent to #323230 in RGB
        Pen borderPen = new Pen(Brushes.Black, 0.15);
        Pen sideBorderPen = new Pen(Brushes.Aqua, 1); // Pen for left and right borders

        foreach (double position in _rectanglePositions)
        {
            double renderHeight = position;
            
            // Draw the rectangle (page break).
            Rect adornedElementRect = new Rect(new Point(0, renderHeight), new Size(AdornedElement.RenderSize.Width, 7));
            drawingContext.DrawRectangle(fillBrush, null, adornedElementRect);
            
            // Top and bottom 'border' drawn to be black
            drawingContext.DrawLine(borderPen, new Point(adornedElementRect.Left, adornedElementRect.Top), new Point(adornedElementRect.Right, adornedElementRect.Top));
            drawingContext.DrawLine(borderPen, new Point(adornedElementRect.Left, adornedElementRect.Bottom), new Point(adornedElementRect.Right, adornedElementRect.Bottom));
            
            // Draw colored borders on the left and right
            drawingContext.DrawLine(sideBorderPen, new Point(adornedElementRect.Left, adornedElementRect.Top), new Point(adornedElementRect.Left, adornedElementRect.Bottom));
            drawingContext.DrawLine(sideBorderPen, new Point(adornedElementRect.Right, adornedElementRect.Top), new Point(adornedElementRect.Right, adornedElementRect.Bottom));
        }
    }


    public void AddNewRectangle(double newPosition)
    {
        _rectanglePositions.Add(newPosition);
        Update(); // Redraw adorner to include new rectangle
    }
}