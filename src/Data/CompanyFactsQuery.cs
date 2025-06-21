using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace SecuritiesExchangeCommission.Edgar.Data
{
    public class CompanyFactsQuery
    {
        public int CIK { get; set; }
        public string EntityName { get; set; }
        public Fact[] Facts { get; set; }

        public static CompanyFactsQuery Parse(JObject jo)
        {
            CompanyFactsQuery ToReturn = new CompanyFactsQuery();

            if (jo.TryGetValue("cik", out JToken val_cik)) { ToReturn.CIK = Convert.ToInt32(val_cik.ToString()); }
            if (jo.TryGetValue("entityName", out JToken val_entityName)) { ToReturn.EntityName = val_entityName.ToString(); }

            //Collect Facts
            List<Fact> Facts = new List<Fact>();
            JProperty prop_facts = jo.Property("facts");
            if (prop_facts != null)
            {
                JObject facts = (JObject)prop_facts.Value;
                foreach (JProperty prop_facttype in facts.Properties()) //i.e. "dei" and "us-gaap"
                {
                    JObject facttype = (JObject)prop_facttype.Value;
                    foreach (JProperty prop_fact in facttype.Properties()) //i.e. "AccountsPayableCurrent", "Assets", "CurrentAssets", etc.
                    {
                        JObject fact = (JObject)prop_fact.Value;
                        Fact ThisFact = Fact.Parse(fact);
                        ThisFact.Tag = prop_fact.Name; //add the name
                        Facts.Add(ThisFact); //Parse and add
                    }
                }
            }
            ToReturn.Facts = Facts.ToArray();

            return ToReturn;
        }

        public static async Task<CompanyFactsQuery> QueryAsync(int CIK)
        {
            string urlPortion = CIK.ToString("0000000000"); //must have leading 0's
            string url = "https://data.sec.gov/api/xbrl/companyfacts/CIK" + urlPortion + ".json";

            //Call
            HttpClient hc = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Get;
            req.RequestUri = new Uri(url);
            req.Headers.Add("User-Agent", RequestManager.Instance.ToUserAgent());
            HttpResponseMessage resp = await hc.SendAsync(req);

            string content = await resp.Content.ReadAsStringAsync();
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Request to SEC data API returned response '" + resp.StatusCode.ToString() + "'. Content: " + content);
            }

            //Parse and Return
            JObject jo = JObject.Parse(content);
            return CompanyFactsQuery.Parse(jo);
        }

    }
}