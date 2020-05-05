using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarSearchResult
    {
        public string Filing { get; set; }
        public string DocumentsUrl { get; set; }
        public string InteractiveDataUrl { get; set; }
        public string Description { get; set; }
        public DateTime FilingDate { get; set; }

        public async Task<Stream> DownloadXbrlDocumentAsync()
        {
            if (DocumentsUrl == "")
            {
                throw new Exception("Documents URL is blank.");
            }

            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(DocumentsUrl);
            string web = await hrm.Content.ReadAsStringAsync();

            int loc1 = 0;
            int loc2 = 0;

            loc2 = web.LastIndexOf("</table>");
            loc1 = web.LastIndexOf("<table", loc2);
            string table_data = web.Substring(loc1, loc2 - loc1);
            string XBRL_URL = "";


            if (table_data.ToLower().Contains("extracted"))
            {
                loc1 = web.LastIndexOf("<tr", loc2);
                string LastRow = web.Substring(loc1, loc2 - loc1);
                if (LastRow.ToLower().Contains("xbrl") == false)
                {
                    throw new Exception("Unable to find XBRL document.");
                }
                loc1 = LastRow.IndexOf("href");
                if (loc1 == -1)
                {
                    throw new Exception("Unable to find download link for XBRL instance document.");
                }
                loc1 = LastRow.IndexOf("\"", loc1);
                loc2 = LastRow.IndexOf("\"", loc1 + 1);
                XBRL_URL = LastRow.Substring(loc1 + 1, loc2 - loc1 - 1);
                XBRL_URL = "https://www.sec.gov" + XBRL_URL;
            }
            else //It is in the top row
            {
                loc1 = table_data.IndexOf("</tr>");
                loc1 = table_data.IndexOf("href", loc1 + 1);
                if (loc1 == -1)
                {
                    throw new Exception("Unable to find download link for XBRL instance document.");
                }
                loc1 = table_data.IndexOf("\"", loc1 + 1);
                loc2 = table_data.IndexOf("\"", loc1 + 1);
                XBRL_URL = table_data.Substring(loc1 + 1, loc2 - loc1 - 1);
                XBRL_URL = "https://www.sec.gov" + XBRL_URL;
            }



            HttpResponseMessage hrmd = await hc.GetAsync(XBRL_URL);
            Stream ds = await hrmd.Content.ReadAsStreamAsync();

            return ds;


        }

    }
}
