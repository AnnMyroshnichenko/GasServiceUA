namespace GasServiceUA.Services
{
    public interface ICurrencyConverterService
    {
        string GetCurrencyExchange(string fromCurrency, string toCurrency, string sum);
    }
}
