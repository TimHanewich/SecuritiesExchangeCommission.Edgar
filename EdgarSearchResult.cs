using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

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

        public async Task<FilingDocument[]> GetDocumentFormatFilesAsync()
        {
            CheckDocumentUrlValid();

            int loc1 = 0;
            int loc2 = 0;
            
            
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(DocumentsUrl);
            string web = await hrm.Content.ReadAsStringAsync();


            loc1 = web.IndexOf("Document Format Files");
            loc2 = web.IndexOf("</table>");
            if (loc2 <= loc1)
            {
                throw new Exception("Unable to locate document format files.");
            }

        
            string docformatfiledata = web.Substring(loc1+1, loc2-loc1-1);
            FilingDocument[] docs = GetDocumentsFromTable(docformatfiledata);

            return docs;
        }
    
        public async Task<FilingDocument[]> GetDataFilesAsync()
        {
            CheckDocumentUrlValid();

            int loc1 = 0;
            int loc2 = 0;
            
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync(DocumentsUrl);
            string web = await hrm.Content.ReadAsStringAsync();

            loc1 = web.IndexOf("Data Files");
            loc2 = web.IndexOf("</table>", loc1 + 1);
            if (loc2 <= loc1)
            {
                throw new Exception("Unable to locate data files.");
            }

            string datafiles = web.Substring(loc1+1, loc2-loc1-1);
            FilingDocument[] docs = GetDocumentsFromTable(datafiles);

            return docs;
        }

        private void CheckDocumentUrlValid()
        {
            //Check for invalid document URL
            if (DocumentsUrl == null)
            {
                throw new Exception("Documents URL was null!  Unable to gather files.");
            }
            if (DocumentsUrl == "")
            {
                throw new Exception("Documents URL was blank!  Unable to gather files.");
            }
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
            Console.WriteLine("Rows: " + rows.Length.ToString());
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
                    loc2 = cols[2].IndexOf("<", loc1 + 1);
                    fd.Description = cols[2].Substring(loc1 + 1, loc2 - loc1 - 1);
                    

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
