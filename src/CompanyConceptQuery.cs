using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritiesExchangeCommission.Edgar.Data
{
    public class CompanyConceptQuery
    {
        public int CIK { get; set; }
        public string EntityName { get; set; }
        public Fact Result { get; set; }

        public static CompanyConceptQuery Parse(JObject jo)
        {
            CompanyConceptQuery ToReturn = new CompanyConceptQuery();

            //Get CIK
            JProperty prop_cik = jo.Property("cik");
            if (prop_cik != null)
            {
                ToReturn.CIK = Convert.ToInt32(prop_cik.Value.ToString());
            }

            //Get entity name
            JProperty prop_entityName = jo.Property("entityName");
            if (prop_entityName != null)
            {
                ToReturn.EntityName = prop_entityName.Value.ToString();
            }

            ToReturn.Result = Fact.Parse(jo);

            return ToReturn;
        }

        public static async Task<CompanyConceptQuery> QueryAsync(int CIK, string tag)
        {
            string cikPortion = CIK.ToString("0000000000"); //must have leading 0's
            string url = "https://data.sec.gov/api/xbrl/companyconcept/CIK" + cikPortion + "/us-gaap/" + tag + ".json";

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
            return CompanyConceptQuery.Parse(jo);
        }
    }
}