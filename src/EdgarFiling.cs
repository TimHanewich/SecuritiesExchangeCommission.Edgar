using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarFiling
    {
        public string Filing {get; set;}
        public string DocumentsUrl {get; set;}
        public string Description {get; set;}
        public DateTime FilingDate {get; set;}

        public async Task<long> GetCikAsync()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(DocumentsUrl);
            string content = await hrm.Content.ReadAsStringAsync();

            int loc1 = content.IndexOf("<acronym title=\"Central Index Key\">CIK</acronym>");
            if (loc1 == -1)
            {
                throw new Exception("Unable to find CIK label in filing.");
            }
            try
            {
                loc1 = content.IndexOf("a href", loc1 + 1);
                loc1 = content.IndexOf(">", loc1 + 1);
                int loc2 = content.IndexOf(" ", loc1 + 1);
                string cik_str = content.Substring(loc1 + 1, loc2 - loc1 - 1).Trim();
                long ToReturn = Convert.ToInt64(cik_str);
                return ToReturn;
            }
            catch
            {
                throw new Exception("Fatal error while trying to find CIK in filing at " + DocumentsUrl);
            }
        }
    }
}