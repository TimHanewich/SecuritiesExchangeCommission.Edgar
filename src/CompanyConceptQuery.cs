using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
    }
}