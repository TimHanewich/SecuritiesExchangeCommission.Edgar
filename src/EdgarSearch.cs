using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarSearch
    {
        public EdgarSearchResult[] Results { get; set; }

        //The values that were searched for
        private string StockSymbol;
        private string FilingType;
        private EdgarSearchOwnershipFilter OwnershipFilter;
        private EdgarSearchResultsPerPage ResultsPerPage;

        //The next search link
        private string NextPageUrl;

        private void ParseFromWebHtml(string web)
        {
            //Error checking
            if (web.Contains("No matching Ticker Symbol."))
            {
                throw new Exception("No matching Ticker Symbol.");
            }

            int loc1 = 0;
            int loc2 = 0;
            List<string> Splitter = new List<string>();

            //Parse into search results
            List<EdgarSearchResult> FilingResults = new List<EdgarSearchResult>();
            loc1 = web.IndexOf("tableFile2");
            loc2 = web.IndexOf("</table>", loc1 + 1);
            if (loc1 == -1 || loc2 == -1) //If we couldnt find the data file table, it means there are no filings, so return nothing.
            {
                Results = FilingResults.ToArray();
                NextPageUrl = null;
                return;
            }
            string resulttable = web.Substring(loc1, loc2 - loc1);
            Splitter.Clear();
            Splitter.Add("<tr");
            string[] rows = resulttable.Split(Splitter.ToArray(), StringSplitOptions.None);
            int t = 0;
            for (t = 2; t < rows.Length; t++)
            {

                //Split to columns
                Splitter.Clear();
                Splitter.Add("<td");
                string[] cols = rows[t].Split(Splitter.ToArray(), StringSplitOptions.None);

                //If this is actually a result
                if (cols.Length > 0)
                {
                    EdgarSearchResult esr = new EdgarSearchResult();

                    //Filing
                    loc1 = cols[1].IndexOf(">");
                    loc2 = cols[1].IndexOf("<", loc1 + 1);
                    esr.Filing = cols[1].Substring(loc1 + 1, loc2 - loc1 - 1);

                    //Documents Button URL's
                    loc1 = cols[2].IndexOf("documentsbutton");
                    if (loc1 != -1)
                    {
                        loc1 = cols[2].LastIndexOf("href", loc1);
                        loc1 = cols[2].IndexOf("\"", loc1 + 1);
                        loc2 = cols[2].IndexOf("\"", loc1 + 1);
                        esr.DocumentsUrl = "https://www.sec.gov" + cols[2].Substring(loc1 + 1, loc2 - loc1 - 1);
                    }

                    //Interactive Content Button URL's
                    loc1 = cols[2].IndexOf("interactiveDataBtn");
                    if (loc1 != -1)
                    {
                        loc1 = cols[2].LastIndexOf("href", loc1);
                        loc1 = cols[2].IndexOf("\"", loc1 + 1);
                        loc2 = cols[2].IndexOf("\"", loc1 + 1);
                        esr.InteractiveDataUrl = "https://www.sec.gov" + cols[2].Substring(loc1 + 1, loc2 - loc1 - 1);
                        esr.InteractiveDataUrl = esr.InteractiveDataUrl.Replace("&amp;", "&");
                    }

                    //Get description
                    loc1 = cols[3].IndexOf(">");
                    loc2 = cols[3].IndexOf("</td");
                    esr.Description = cols[3].Substring(loc1 + 1, loc2 - loc1 - 1);

                    //Get filing date
                    loc1 = cols[4].IndexOf(">");
                    loc2 = cols[4].IndexOf("<", loc1 + 1);
                    esr.FilingDate = DateTime.Parse(cols[4].Substring(loc1 + 1, loc2 - loc1 - 1));

                    FilingResults.Add(esr);
                }
            }
            Results = FilingResults.ToArray();
        
            //Get the next button if it exists
            string search_for = "<input type=\"button\" value=\"Next ";
            loc1 = web.IndexOf(search_for);
            if (loc1 != -1)
            {
                loc1 = web.IndexOf(".location", loc1 + 1);
                loc1 = web.IndexOf("'", loc1 + 1);
                loc2 = web.IndexOf("'", loc1 + 1);
                string nexturl = web.Substring(loc1 + 1, loc2 - loc1 - 1);
                nexturl = "https://www.sec.gov" + nexturl;
                NextPageUrl = nexturl;
            }
            else
            {
                NextPageUrl = null;
            }
        
        }

        public static async Task<EdgarSearch> CreateAsync(string stock_symbol, string filing_type = "", DateTime? prior_to = null, EdgarSearchOwnershipFilter ownership_filter = EdgarSearchOwnershipFilter.exclude, EdgarSearchResultsPerPage results_per_page = EdgarSearchResultsPerPage.Entries40)
        {

            EdgarSearch es = new EdgarSearch();

            #region "Save search parameters"

            es.StockSymbol = stock_symbol;
            es.FilingType = filing_type;
            es.OwnershipFilter = ownership_filter;
            es.ResultsPerPage = results_per_page;

            #endregion

            #region "Construct the query URL"

            string URL = "https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany";

            //Add the stock symbol
            URL = URL + "&CIK=" + stock_symbol;

            //Filing type?
            if (filing_type != "")
            {
                URL = URL + "&type=" + filing_type;
            }

            //Prior to?
            if (prior_to != null)
            {
                string fs = prior_to.Value.Year.ToString("0000") + prior_to.Value.Month.ToString("00") + prior_to.Value.Day.ToString("00");
                URL = URL + "&dateb=" + fs;
            }

            //Edgar ownership
            if (ownership_filter == EdgarSearchOwnershipFilter.exclude)
            {
                URL = URL + "&owner=exclude";
            }
            else if (ownership_filter == EdgarSearchOwnershipFilter.include)
            {
                URL = URL + "&owner=include";
            }
            else if (ownership_filter == EdgarSearchOwnershipFilter.only)
            {
                URL = URL + "&owner=only";
            }

            //Results per page
            if (results_per_page == EdgarSearchResultsPerPage.Entries10)
            {
                URL = URL + "&count=10";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries20)
            {
                URL = URL + "&count=20";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries40)
            {
                URL = URL + "&count=40";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries80)
            {
                URL = URL + "&count=80";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries100)
            {
                URL = URL + "&count=100";
            }
            #endregion

            //Get web data
            string web = await SecRequestManager.Instance.SecGetAsync(URL);

            //Now load and return the data
            es.ParseFromWebHtml(web);
            return es;          
        }

        public EdgarSearchResult GetFirstResultOfFilingType(string filing)
        {
            foreach (EdgarSearchResult esr in Results)
            {
                if (esr.Filing.ToLower() == filing.ToLower())
                {
                    return esr;
                }
            }
            throw new Exception("Unable to find result of filing type '" + filing + "' in the search results.");
        }
    
        public async Task<EdgarSearch> NextPageAsync()
        {
            if (NextPageUrl == null)
            {
                throw new Exception("There is not another page to request");
            }

            string content = await SecRequestManager.Instance.SecGetAsync(NextPageUrl);

            EdgarSearch es = new EdgarSearch();
            es.ParseFromWebHtml(content);
            return es;
        }
    
        public bool NextPageAvailable()
        {
            if (NextPageUrl == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
