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
            SecRequestManager reqmgr = new SecRequestManager();
            string web = await reqmgr.SecGetAsync(url);

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
    
        public static HttpRequestMessage PrepareHttpRequestMessage()
        {
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Get;
            req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36 Edg/89.0.774.75"); //This identifies the request as coming from a browser. If we do not provide info, the SEC will flag this as coming from an undeclared tool.
            return req;
        }
    }
}