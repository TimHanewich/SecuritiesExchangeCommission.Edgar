using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace SecuritiesExchangeCommission.Edgar
{
    public class SecToolkit
    {
        public static async Task<string> GetCompanyCikFromTradingSymbolAsync(string symbol)
        {
            string url = "https://www.sec.gov/cgi-bin/browse-edgar?CIK=" + symbol + "&owner=exclude";
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(url);
            string web = await hrm.Content.ReadAsStringAsync();

            //If it isn't found
            if (web.Contains("<h1>No matching Ticker Symbol.</h1>"))
            {
                throw new Exception("No matching ticker symbol in the SEC database for '" + symbol + "'.");
            }

            int loc1 = web.IndexOf(">CIK</acronym>");
            loc1 = web.IndexOf("href", loc1 + 1);
            loc1 = web.IndexOf(">", loc1 + 1);
            int loc2 = web.IndexOf(" ", loc1 + 1);
            string cikstr = web.Substring(loc1 + 1, loc2 - loc1 - 1).Trim();
            return cikstr;
        }
    }
}