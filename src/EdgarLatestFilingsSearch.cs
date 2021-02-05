using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarLatestFilingsSearch
    {
        public EdgarLatestFilingResult[] Results {get; set;}

        public static async Task<EdgarLatestFilingsSearch> SearchAsync(string form_type = null, EdgarSearchOwnershipFilter ownership_filter = EdgarSearchOwnershipFilter.include, EdgarSearchResultsPerPage results_per_page = EdgarSearchResultsPerPage.Entries40)
        {
            EdgarLatestFilingsSearch ToReturn = new EdgarLatestFilingsSearch();

            #region "Make the search URL"

            string search_url = "https://www.sec.gov/cgi-bin/browse-edgar?";

            //Form type
            if (form_type != null)
            {
                search_url = search_url + "&type=" + form_type;
            }

            //Owner
            if (ownership_filter == EdgarSearchOwnershipFilter.exclude)
            {
                search_url = search_url + "&owner=exclude";
            }
            else if (ownership_filter == EdgarSearchOwnershipFilter.include)
            {
                search_url = search_url + "&owner=include";
            }
            else if (ownership_filter == EdgarSearchOwnershipFilter.only)
            {
                search_url = search_url + "&owner=only";
            }

            //Results per page
            if (results_per_page == EdgarSearchResultsPerPage.Entries10)
            {
                search_url = search_url + "&count=10";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries20)
            {
                search_url = search_url + "&count=20";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries40)
            {
                search_url = search_url + "&count=40";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries80)
            {
                search_url = search_url + "&count=80";
            }
            else if (results_per_page == EdgarSearchResultsPerPage.Entries100)
            {
                search_url = search_url + "&count=100";
            }
            
            //Add the get current
            search_url = search_url + "&action=getcurrent";

            #endregion
        
            //Call the search
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(search_url);
            string content = await hrm.Content.ReadAsStringAsync();

            List<EdgarLatestFilingResult> searchResults = new List<EdgarLatestFilingResult>();

            //Is it no matching? If so, return an empty array
            if (content.ToLower().Contains("no matching filings"))
            {
                ToReturn.Results = searchResults.ToArray();
                return ToReturn;
            }

            int loc1;
            int loc2;
            List<string> Splitter = new List<string>();

            //Get the tbody
            loc1 = content.IndexOf("File/Film No");
            loc2 = content.IndexOf("</table>", loc1 + 1);
            string table_data = content.Substring(loc1, loc2 - loc1);

            //Get a list of all the rows with the titles
            Splitter.Clear();
            Splitter.Add("<tr>");
            string[] rows_data_titles = table_data.Split(Splitter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            List<string> Titles = new List<string>();
            foreach (string s in rows_data_titles)
            {
                loc1 = s.IndexOf("<td bg");
                loc1 = s.IndexOf("href", loc1 + 1);
                loc1 = s.IndexOf(">", loc1 + 1);
                loc2 = s.IndexOf("<", loc1 + 1);
                string thistitle = s.Substring(loc1 + 1, loc2 - loc1 - 1);
                Titles.Add(thistitle);
            }
            Titles.RemoveAt(0); //Remove the first one because that is blank.
            

            //Split into rows
            List<EdgarLatestFilingResult> ESRs = new List<EdgarLatestFilingResult>();
            Splitter.Clear();
            Splitter.Add("<tr nowrap");
            string[] rows_data = table_data.Split(Splitter.ToArray(), StringSplitOptions.None);
            for (int t = 1; t < rows_data.Length; t++)
            {
                EdgarLatestFilingResult esr = new EdgarLatestFilingResult();
                string rowdata = rows_data[t];

                //Get the entity name and CIK from this title
                string thistitle = Titles[t-1]; //-1 because this loop starts at 1 and the titles are clean (every one is legitimate)
                loc1 = thistitle.IndexOf("(");
                string this_ent_title = thistitle.Substring(0, loc1 - 1).Trim();
                esr.EntityTitle = this_ent_title;

                //Get the CIK
                loc2 = thistitle.IndexOf(")", loc1 + 1);
                string this_cik = thistitle.Substring(loc1 + 1, loc2 - loc1 - 1).Trim();
                try
                {
                    esr.EntityCik = Convert.ToInt32(this_cik);
                }
                catch
                {
                    esr.EntityCik = 0;
                }
                        
                //Split into columns
                Splitter.Clear();
                Splitter.Add("<td");
                string[] columns_data = rowdata.Split(Splitter.ToArray(), StringSplitOptions.None);

                //Get filing type (1st column)
                loc1 = columns_data[1].IndexOf(">");
                loc2 = columns_data[1].IndexOf("<", loc1 + 1);
                esr.Filing = columns_data[1].Substring(loc1 + 1, loc2 - loc1 - 1);

                //Documents link
                loc1 = columns_data[2].IndexOf("a href");
                loc1 = columns_data[2].IndexOf("\"", loc1 + 1);
                loc2 = columns_data[2].IndexOf("\"", loc1 + 1);
                string documentsURL = columns_data[2].Substring(loc1 + 1, loc2 - loc1 - 1);
                documentsURL = "https://www.sec.gov" + documentsURL;
                esr.DocumentsUrl = documentsURL;

                //Description (index 3, column 3)
                loc1 = columns_data[3].IndexOf(">");
                loc2 = columns_data[3].IndexOf("</td");
                string desc = columns_data[3].Substring(loc1 + 1, loc2 - loc1 - 1);
                desc = desc.Replace("<br>", " ");
                desc = desc.Replace("&nbsp;", " ");
                esr.Description = desc;

                //Filing date (index 5)
                loc1 = columns_data[5].IndexOf(">");
                loc2 = columns_data[5].IndexOf("<", loc1 + 1);
                esr.FilingDate = DateTime.Parse(columns_data[5].Substring(loc1 + 1, loc2 - loc1 - 1));

                ESRs.Add(esr);
            }
            ToReturn.Results = ESRs.ToArray();

            return ToReturn;
        }
    }
}