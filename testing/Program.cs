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
            GatherAll(args);
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

        public static void MiscTest()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = hc.GetAsync("https://www.sec.gov/cgi-bin/browse-edgar?company=&CIK=&type=10-K&owner=include&count=40&action=getcurrent").Result;
            string content = hrm.Content.ReadAsStringAsync().Result;
            System.IO.File.WriteAllText("C:\\Users\\tihanewi\\Downloads\\WEB2.txt", content);
        }

        public static void GatherAll(string[] args)
        {
            List<EdgarSearchResult> RESULTS = new List<EdgarSearchResult>();
            bool Kill = false;
            EdgarSearch es = EdgarSearch.CreateAsync(args[0], "4", null, EdgarSearchOwnershipFilter.only).Result;
            while (Kill == false)
            {
                Console.WriteLine("Adding...");
                foreach (EdgarSearchResult esr in es.Results)
                {
                    RESULTS.Add(esr);
                }
                
                //Paging
                Console.Write("Getting next page... ");
                es = es.NextPageAsync().Result;
                if (es.Results.Length == 0)
                {
                    Console.WriteLine("ITS OVER!");
                    Kill = true;
                }
                else
                {
                    Console.WriteLine("Got it");
                }
            }

            Console.WriteLine("Got all of them: " + RESULTS.Count.ToString());
        }

    }
}
