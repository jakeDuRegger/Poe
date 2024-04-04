using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Size = System.Drawing.Size;

namespace Poe.Models
{
    // possible answer: https://stackoverflow.com/questions/5107880/wpf-fixeddocument-pagination 
    // interesting as well: https://www.codeproject.com/Articles/31834/FlowDocument-pagination-with-repeating-page-header 
    // https://github.com/sherman89/WpfReporting - Repo concerning wpf pagination / printing which is a big big deal!
    // If I could get printing that would be stellar....
    public class Page
    {
        // Just need to have set document sizes
        // That's exactly what word does
        // I.e. only let like 8.5 x 11 pages... etc,
        // Maybe the UI should be in a different format then what is actually printed
        public FlowDocument Content { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        
        
        public static List<Page> Paginate(FlowDocument originalDocument, double pageHeight)
        {
            List<Page> pages = new List<Page>();
            FlowDocument currentPageDocument = new FlowDocument();
            if (originalDocument.Blocks.FirstBlock != null)
            {
                Block currentBlock = originalDocument.Blocks.FirstBlock;

                while (currentBlock != null)
                {
                    // Clone the current block and add it to the current page document
                    Block clonedBlock = CloneBlock(currentBlock);
                    currentPageDocument.Blocks.Add(clonedBlock);

                    // Check if adding this block exceeded the page height
                    if (MeasureDocumentHeight(currentPageDocument) > pageHeight)
                    {
                        // Remove the block that caused overflow and finalize the current page
                        currentPageDocument.Blocks.Remove(clonedBlock);
                        pages.Add(new Page { Content = currentPageDocument, Height = pageHeight });

                        // Start a new page
                        currentPageDocument = new FlowDocument();
                    }
                    else
                    {
                        // Move to the next block in the original document
                        if (currentBlock.NextBlock != null) currentBlock = currentBlock.NextBlock;
                    }
                }
            }

            // Add the last page if it has any content
            if (currentPageDocument.Blocks.Count > 0)
            {
                pages.Add(new Page { Content = currentPageDocument, Height = pageHeight });
            }

            return pages;
        }

        private static Block CloneBlock(Block originalBlock)
        {
            // Implement cloning for different block types as needed
            // This is a simplified example for Paragraph
            if (originalBlock is Paragraph originalParagraph)
            {
                Paragraph clonedParagraph = new Paragraph();
                foreach (Inline inline in originalParagraph.Inlines)
                {
                    if (inline is Run originalRun)
                    {
                        clonedParagraph.Inlines.Add(new Run(originalRun.Text));
                    }
                }
                return clonedParagraph;
            }

            // Add cloning for other block types (e.g., Table, List, Section)

            return null; // Or throw an exception for unsupported block types
        }

        private static double MeasureDocumentHeight(FlowDocument document)
        {
            // Implement a method to measure the height of the document content
            // This can be done using a FlowDocumentScrollViewer or other approach
            // For simplicity, this method is not fully implemented here
            return 0;
        }
        
        
        
        
        
        
        
        
        
        
        
    }

}