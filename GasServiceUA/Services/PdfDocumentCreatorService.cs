
using SelectPdf;
using TallComponents.PDF.Forms.Data;

namespace GasServiceUA.Services
{
    public class PdfDocumentCreatorService : IDocumentCreatorService
    {
        public byte[] CreateDocument(string viewHtml, string baseUrl)
        {
            HtmlToPdf htmlToPdf = new HtmlToPdf();
            PdfDocument pdfDocument = htmlToPdf.ConvertHtmlString(viewHtml, baseUrl);
            byte[] pdf = pdfDocument.Save();
            pdfDocument.Close();
            return pdf;
        }
    }
}
