using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarSearchResult : EdgarFiling
    {
        public string InteractiveDataUrl { get; set; }

        public async Task<Stream> DownloadXbrlDocumentAsync()
        {
            FilingDocument[] DataFiles = await GetDataFilesAsync();

            //Try and get it from the document filing description
            foreach (FilingDocument fd in DataFiles)
            {
                if (fd.Description.Trim().ToLower().Contains("instance document"))
                {
                    SecRequestManager reqmgr = new SecRequestManager();
                    Stream s = await reqmgr.SecGetStreamAsync(fd.Url);
                    return s;
                }
            }

            //Try and get it from the document filing type
            foreach (FilingDocument fd in DataFiles)
            {
                if (fd.DocumentType.ToLower().Contains("ins"))
                {
                    SecRequestManager reqmgr = new SecRequestManager();
                    Stream s = await reqmgr.SecGetStreamAsync(fd.Url);
                    return s;
                }
            }

            throw new Exception("Unable to find XBRL Instance Document in this filing document.");
        }

        public async Task<FilingDocument[]> GetDocumentFormatFilesAsync()
        {
            CheckDocumentUrlValid();

            int loc1 = 0;
            int loc2 = 0;
            
            SecRequestManager reqmgr = new SecRequestManager();
            string web = await reqmgr.SecGetAsync(DocumentsUrl);

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
            
            SecRequestManager reqmgr = new SecRequestManager();
            string web = await reqmgr.SecGetAsync(DocumentsUrl);

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

        public async Task<string> GetAccessionNumberAsync()
        {
            CheckDocumentUrlValid();

            SecRequestManager reqmgr = new SecRequestManager();
            string web = await reqmgr.SecGetAsync(DocumentsUrl);

            int loc1 = web.IndexOf("<div id=\"secNum\">");
            if (loc1 == -1)
            {
                throw new Exception("Unable to locate the Accession Number. It may not exist!");
            }
            loc1 = web.IndexOf("</strong>", loc1 + 1);
            loc1 = web.IndexOf(">", loc1 + 1);
            int loc2 = web.IndexOf("<", loc1 + 1);
            string acn = web.Substring(loc1 + 1, loc2 - loc1 - 1);
            acn = acn.Trim();
            return acn;
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
