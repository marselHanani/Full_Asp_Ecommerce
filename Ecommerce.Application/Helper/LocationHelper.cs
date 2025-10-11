using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.Application.Helper
{
    public class LocationHelper(HttpClient http)
    {
        private readonly HttpClient _http = http;

        public async Task<string> GetCurrencyByIpAsync(string ip)
        {
            try
            {
                var response = await _http.GetAsync($"https://ipapi.co/{ip}/json/");
                if (!response.IsSuccessStatusCode) return "usd";

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("currency", out var currency))
                {
                    var code = currency.GetString();
                    if (!string.IsNullOrWhiteSpace(code))
                        return code.ToLower();
                }

                return "usd";
            }
            catch
            {
                return "usd";
            }
        }
    }
}
