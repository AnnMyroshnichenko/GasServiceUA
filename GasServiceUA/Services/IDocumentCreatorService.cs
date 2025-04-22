namespace GasServiceUA.Services
{
    public interface IDocumentCreatorService
    {
        byte[] CreateDocument(string viewHtml, string baseUrl);
    }
}
