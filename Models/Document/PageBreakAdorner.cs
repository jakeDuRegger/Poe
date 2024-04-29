using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Poe.Models.Document;

public class PageBreakAdorner : Adorner
{
    internal double PageBreakHeight { get; set; } // todo come back later and figure out why we need / if need internal

    internal List<double> RectanglePositions;

    public PageBreakAdorner(UIElement adornedElement, double documentHeight) : base(adornedElement)
    {
        PageBreakHeight = documentHeight + 10;
        IsHitTestVisible = false; // Makes sure adorner doesn't interfere with text editing
        RectanglePositions = new List<double> { PageBreakHeight }; // Add the original at page break height.
    }
    
    public void UpdateRectanglePositions(List<double> validBreaks)
    {
        RectanglePositions = validBreaks;
        Update(); // Redraw the adorner with the updated breaks
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
        Pen sideBorderPen = new Pen(new SolidColorBrush(Color.FromRgb(50, 50, 48)), 1); // Pen for left and right borders

        foreach (double position in RectanglePositions)
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
        RectanglePositions.Add(newPosition);
        Update(); // Redraw adorner to include new rectangle
    }

    public void RemoveRectangleAt(int index)
    {
        if (index >= 0 && index < RectanglePositions.Count)
        {
            RectanglePositions.RemoveAt(index);
            Update();
        }
    }
}