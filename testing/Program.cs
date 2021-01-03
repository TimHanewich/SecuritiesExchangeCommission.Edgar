using System;
using System.IO;
using SecuritiesExchangeCommission.Edgar;
using SecuritiesExchangeCommission;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            // string content = System.IO.File.ReadAllText("C:\\Users\\tihanewi\\Downloads\\Form4\\bac.xml");
            // StatementOfChangesInBeneficialOwnership form4 = StatementOfChangesInBeneficialOwnership.ParseXml(content);
            // Console.WriteLine(JsonConvert.SerializeObject(form4));


            HttpClient hc = new HttpClient();
            EdgarSearch es = EdgarSearch.CreateAsync("snap", "4", null, EdgarSearchOwnershipFilter.only).Result;
            foreach (EdgarSearchResult esr in es.Results)
            {
                if (esr.Filing == "4")
                {
                    FilingDocument[] docs = esr.GetDocumentFormatFilesAsync().Result;
                    foreach (FilingDocument fd in docs)
                    {
                        if (fd.DocumentName.ToLower().Contains(".xml"))
                        {
                            Console.WriteLine("Tryng to get " + fd.Url + " ...");
                            HttpResponseMessage hrm = hc.GetAsync(fd.Url).Result;
                            string content = hrm.Content.ReadAsStringAsync().Result;
                            StatementOfChangesInBeneficialOwnership form4 = StatementOfChangesInBeneficialOwnership.ParseXml(content);
                        }
                    }
                }
            }

        }
    }
}
