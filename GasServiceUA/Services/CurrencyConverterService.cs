using System.Globalization;
using System.Net;
using System.Text.Json;
using currencyapi;
using DotNetEnv;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GasServiceUA.Services
{
    public class CurrencyConverterService : ICurrencyConverterService
    {

        public string GetCurrencyExchange(string fromCurrency, string toCurrency, string sum)
        {
            try
            {
                var converter = new Currencyapi(Environment.GetEnvironmentVariable("CURRENCY_CONVERTER_APIKEY"));
                var exchangeRate = converter.Latest(fromCurrency, toCurrency);
                
                JsonDocument jsonDoc = JsonDocument.Parse(exchangeRate);
                JsonElement root = jsonDoc.RootElement;
                JsonElement dataElement = root.GetProperty("data");
                JsonElement usdElement = dataElement.GetProperty(toCurrency);
                decimal value = usdElement.GetProperty("value").GetDecimal();

                var convertedSum = Math.Round(Convert.ToDecimal(sum) * value, 2);
                
                return convertedSum.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }

            return sum;
        }

    }
}
