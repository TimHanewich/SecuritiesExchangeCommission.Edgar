using System;
using System.IO;
using SecuritiesExchangeCommission.Edgar;
using SecuritiesExchangeCommission;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using TimHanewich.Investing;
using System.Collections.Generic;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            IndividualTest(args);
        }

        public static void FullSp500Test()
        {
            Console.WriteLine("Getting sp500...");
            string[] sp500 = InvestingToolkit.GetEquityGroupAsync(EquityGroup.SP500).Result;

            HttpClient hc = new HttpClient();

            List<string> Failures = new List<string>();
            int t = 1;
            foreach (string s in sp500)
            {
                Console.Write("Tring " + s + " (" + t.ToString() + "/" + sp500.Length.ToString() + ")... ");
                try
                {
                    EdgarSearch es = EdgarSearch.CreateAsync(s, "4", null, EdgarSearchOwnershipFilter.only).Result;
                    foreach (EdgarSearchResult esr in es.Results)
                    {
                        if (esr.Filing == "4")
                        {
                            FilingDocument[] docs = esr.GetDocumentFormatFilesAsync().Result;
                            foreach (FilingDocument fd in docs)
                            {
                                if (fd.DocumentName.ToLower().Contains(".xml"))
                                {
                                    //Console.WriteLine("Tryng to get " + fd.Url + " ...");
                                    HttpResponseMessage hrm = hc.GetAsync(fd.Url).Result;
                                    string content = hrm.Content.ReadAsStringAsync().Result;
                                    StatementOfChangesInBeneficialOwnership form4 = StatementOfChangesInBeneficialOwnership.ParseXml(content);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Success");
                }
                catch
                {
                    Failures.Add(s);
                    Console.WriteLine("FAILURE!");
                } 
                t = t + 1;
            }

            Console.WriteLine("DONE!");
            Console.WriteLine("Failures:");
            foreach (string s in Failures)
            {
                Console.WriteLine(s);
            }
        }
    
        public static void IndividualTest(string[] args)
        {
            StatementOfChangesInBeneficialOwnership form4 = StatementOfChangesInBeneficialOwnership.ParseXmlFromWebUrlAsync(args[0]).Result;
            Console.WriteLine(JsonConvert.SerializeObject(form4));
        }

    }
}
