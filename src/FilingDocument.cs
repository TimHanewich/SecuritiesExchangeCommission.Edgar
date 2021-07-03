using System;
using System.IO;
using System.Threading.Tasks;

namespace SecuritiesExchangeCommission.Edgar
{
    public class FilingDocument
    {
        public int Sequence {get; set;}
        public string Description {get; set;}
        public string DocumentName {get; set;}
        public string Url {get; set;}
        public string DocumentType {get; set;}
        public int Size {get; set;}

        public async Task<Stream> DownloadAsync()
        {
            //Check a URL exists
            if (Url == null)
            {
                throw new Exception("Unable to download document. Url was null.");
            }
            else if (Url == "")
            {
                throw new Exception("Unable to download document. Url was blank.");
            }

            //Download
            Stream s = null;
            try
            {
                s = await SecRequestManager.Instance.SecGetStreamAsync(Url);
            }
            catch (Exception ex)
            {
                throw new Exception("Fatal failure while downloading filing document at '" + Url + "'. Msg: " + ex.Message);
            }

            return s;
        }
    }
}