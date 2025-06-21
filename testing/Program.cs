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

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestManager.Instance.AppName = "LApp";
            RequestManager.Instance.AppVersion = "1.0";
            RequestManager.Instance.Email = "chrisha@gmail.com";

            CompanyConceptQuery ccq = CompanyConceptQuery.QueryAsync(789019, "AssetsCurrent").Result;
            Console.WriteLine(JsonConvert.SerializeObject(ccq));
        }
    }
}
