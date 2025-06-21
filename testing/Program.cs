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
using SecuritiesExchangeCommission.Edgar.Data;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using SecuritiesExchangeCommission.Edgar;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            IdentificationManager.Instance.AppName = "LApp";
            IdentificationManager.Instance.AppVersion = "1.0";
            IdentificationManager.Instance.Email = "chrisha@gmail.com";

            EdgarSearch msft10ks = EdgarSearch.CreateAsync("MSFT", "10-K").Result;

            Console.WriteLine(JsonConvert.SerializeObject(msft10ks));

        }
    }
}
