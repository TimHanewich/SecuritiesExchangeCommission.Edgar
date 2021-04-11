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
            HttpRequestMessage req = SecToolkit.PrepareHttpRequestMessage();
            req.RequestUri = new Uri(DocumentsUrl);
            HttpResponseMessage hrm = await hc.SendAsync(req);
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
    
        public async Task<EdgarFilingDetails> GetFilingDetailsAsync()
        {
            //Error checking
            if (DocumentsUrl == null || DocumentsUrl == "")
            {
                throw new Exception("Unable to get filings details: documents URL is null or blank.");
            }

            EdgarFilingDetails ToReturn = new EdgarFilingDetails();

            //Get the content
            HttpClient hc = new HttpClient();
            HttpRequestMessage req = SecToolkit.PrepareHttpRequestMessage();
            req.RequestUri = new Uri(DocumentsUrl);
            HttpResponseMessage hrm = await hc.SendAsync(req);
            string web = await hrm.Content.ReadAsStringAsync();

            //Get the accession number
            try
            {
                int loc1 = web.IndexOf("<div id=\"secNum\">");
                if (loc1 != -1)
                {
                    loc1 = web.IndexOf("</strong>", loc1 + 1);
                    loc1 = web.IndexOf(">", loc1 + 1);
                    int loc2 = web.IndexOf("<", loc1 + 1);
                    string acn = web.Substring(loc1 + 1, loc2 - loc1 - 1);
                    acn = acn.Trim();
                    List<string> Splitter = new List<string>();
                    Splitter.Add("-");
                    string[] parts = acn.Split(Splitter.ToArray(), StringSplitOptions.None);
                    ToReturn.AccessionNumberP1 = Convert.ToInt64(parts[0]);
                    ToReturn.AccessionNumberP2 = Convert.ToInt32(parts[1]);
                    ToReturn.AccessionNumberP3 = Convert.ToInt32(parts[2]);
                }
            }
            catch
            {

            }

            //Get the form type
            try
            {
                int loc1 = web.IndexOf("identInfo");
                loc1 = web.IndexOf("Type:", loc1 + 1);
                loc1 = web.IndexOf("<strong>", loc1 + 1);
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf("<", loc1 + 1);
                string form = web.Substring(loc1 + 1, loc2 - loc1 -1);
                ToReturn.Form = form;
            }
            catch
            {

            }

            //Filing Date
            try
            {
                int loc1 = web.IndexOf("<div class=\"infoHead\">Filing Date</div>");
                loc1 = web.IndexOf("Filing Date", loc1 + 1);
                loc1 = web.IndexOf("info", loc1 + 1);
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf("<", loc1 + 1);
                ToReturn.FilingDate = Convert.ToDateTime(web.Substring(loc1 + 1, loc2 - loc1 - 1));
            }
            catch
            {

            }

            //Period of report
            try
            {
                int loc1 = web.IndexOf("<div class=\"infoHead\">Period of Report</div>");
                loc1 = web.IndexOf("Period of Report", loc1 + 1);
                loc1 = web.IndexOf("info", loc1 + 1);
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf("<", loc1 + 1);
                ToReturn.PeriodOfReport = Convert.ToDateTime(web.Substring(loc1 + 1, loc2 - loc1 - 1));
            }
            catch
            {

            }

            //Accepted
            try
            {
                int loc1 = web.IndexOf("<div class=\"infoHead\">Accepted</div>");
                loc1 = web.IndexOf("Accepted", loc1 + 1);
                loc1 = web.IndexOf("info", loc1 + 1);
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf("<", loc1 + 1);
                ToReturn.Accepted = Convert.ToDateTime(web.Substring(loc1 + 1, loc2 - loc1 - 1));
            }
            catch
            {

            }

            //Entity name
            try
            {
                int loc1 = web.IndexOf("companyName");
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf("<", loc1 + 1);
                string company_name = web.Substring(loc1 + 1, loc2 - loc1 - 1);
                company_name = company_name.Replace("(Filer)", "");
                company_name = company_name.Trim();
                ToReturn.EntityName = company_name;
            }
            catch
            {
                
            }

            //Get the cik
            try
            {
                int loc1 = web.IndexOf("<acronym title=\"Central Index Key\">CIK</acronym>");
                loc1 = web.IndexOf("a href", loc1 + 1);
                loc1 = web.IndexOf(">", loc1 + 1);
                int loc2 = web.IndexOf(" ", loc1 + 1);
                string cik_str = web.Substring(loc1 + 1, loc2 - loc1 - 1).Trim();
                ToReturn.EntityCik = Convert.ToInt64(cik_str);
            }
            catch
            {

            }

            //Get the ddocument format files
            try
            {
                int loc1 = 0;
                int loc2 = 0;

                loc1 = web.IndexOf("Document Format Files");
                loc2 = web.IndexOf("</table>");
                if (loc2 <= loc1)
                {
                    throw new Exception("Unable to locate document format files.");
                }

            
                string docformatfiledata = web.Substring(loc1+1, loc2-loc1-1);
                FilingDocument[] docs = GetDocumentsFromTable(docformatfiledata);
                ToReturn.DocumentFormatFiles = docs;
            }
            catch
            {
                
            }

            //Get the data files
            try
            {
                int loc1 = 0;
                int loc2 = 0;

                loc1 = web.IndexOf("Data Files");
                loc2 = web.IndexOf("</table>", loc1 + 1);
                if (loc2 <= loc1)
                {
                    throw new Exception("Unable to locate data files.");
                }

                string datafiles = web.Substring(loc1+1, loc2-loc1-1);
                FilingDocument[] docs = GetDocumentsFromTable(datafiles);
                ToReturn.DataFiles = docs;
            }
            catch
            {

            }

            
            
            return ToReturn;
        }
    
        private FilingDocument[] GetDocumentsFromTable(string table_data)
        {
            int loc1 = 0;
            int loc2 = 0;
            List<FilingDocument> fds = new List<FilingDocument>();
            List<string> splitter = new List<string>();
            splitter.Clear();
            splitter.Add("<tr");
            string[] rows = table_data.Split(splitter.ToArray(), StringSplitOptions.None);
            int r = 0;
            for (r = 2;r<rows.Length;r++)
            {

                   
                splitter.Clear();
                splitter.Add("<td");
                string[] cols = rows[r].Split(splitter.ToArray(), StringSplitOptions.None);
                
                if (cols.Length > 1)
                {
                    FilingDocument fd = new FilingDocument();
                    
                    //Get sequence #
                    loc1 = cols[1].IndexOf(">");
                    loc2 = cols[1].IndexOf("<", loc1 + 1);
                    string seqnum = cols[1].Substring(loc1+1,loc2-loc1 - 1);
                    try
                    {
                        fd.Sequence = Convert.ToInt32(seqnum);
                    }
                    catch
                    {
                        fd.Sequence = 0;
                    }
                    
                    //Get description
                    loc1 = cols[2].IndexOf(">");
                    loc2 = cols[2].IndexOf("</td", loc1 + 1);
                    fd.Description = cols[2].Substring(loc1 + 1, loc2 - loc1 - 1);
                    //Remove the bold symbols
                    fd.Description = fd.Description.Replace("<b>","");
                    fd.Description = fd.Description.Replace("</b>","");
                    

                    //Get URL and file name
                    loc1 = cols[3].IndexOf("href");
                    loc1 = cols[3].IndexOf("\"", loc1 + 1);
                    loc2 = cols[3].IndexOf("\"", loc1 + 1);
                    fd.Url = cols[3].Substring(loc1 + 1, loc2 - loc1 - 1);
                    fd.Url = "https://www.sec.gov/" + fd.Url;
                    loc1 = cols[3].IndexOf(">", loc1 + 1);
                    loc2 = cols[3].IndexOf("<", loc1 + 1);
                    fd.DocumentName = cols[3].Substring(loc1+1,loc2-loc1-1);

                    //Get doc type
                    loc1 = cols[4].IndexOf(">");
                    loc2 = cols[4].IndexOf("<", loc1 + 1);
                    fd.DocumentType = cols[4].Substring(loc1 + 1, loc2-loc1-1);
                    if (fd.DocumentType == "&nbsp;")
                    {
                        fd.DocumentType = "";
                    }


                    //Get doc size
                    loc1 = cols[5].IndexOf(">");
                    loc2 = cols[5].IndexOf("<", loc1 + 1);
                    try
                    {
                        fd.Size = Convert.ToInt32(cols[5].Substring(loc1 + 1, loc2 - loc1 - 1));
                    }
                    catch
                    {
                        fd.Size = 0;
                    }
                    

                    fds.Add(fd);
                }

                
            }
            return fds.ToArray();
        }
    }
}