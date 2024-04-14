using System.Windows.Documents;

namespace Poe.Models.Document
{
    // possible answer: https://stackoverflow.com/questions/5107880/wpf-fixeddocument-pagination 
    // interesting as well: https://www.codeproject.com/Articles/31834/FlowDocument-pagination-with-repeating-page-header 
    // https://github.com/sherman89/WpfReporting - Repo concerning wpf pagination / printing which is a big big deal!
    // If I could get printing that would be stellar....
    public class Page
    {
        public List<Paragraph> Paragraphs { get; set; }

        public Page()
        {
            Paragraphs = new List<Paragraph>();
        }
        
    }


}