using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SecuritiesExchangeCommission.Edgar.Data
{
    public class Fact
    {
        public string Tag { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public FactDataPoint[] DataPoints { get; set; }

        public static Fact Parse(JObject jo)
        {
            Fact ToReturn = new Fact();

            if (jo.TryGetValue("tag", out JToken val_tag)){ ToReturn.Tag = val_tag.ToString(); } //The tag will be in the fact JSON itself in the CompanyConceptQuery only... not CompanyFactsQuery (in that scenario, it is the NAME of the JObject property, so not visible within it)
            if (jo.TryGetValue("label", out JToken val_label)) { ToReturn.Label = val_label.ToString(); }
            if (jo.TryGetValue("description", out JToken val_description)){ ToReturn.Description = val_description.ToString(); }

            //Get fact data
            List<FactDataPoint> DataPoints = new List<FactDataPoint>();
            JProperty prop_units = jo.Property("units");
            if (prop_units != null)
            {
                JObject units = (JObject)prop_units.Value;
                foreach (JProperty prop_unittypes in units.Properties())
                {
                    JArray unittype = (JArray)prop_unittypes.Value;
                    foreach (JObject factdata in unittype)
                    {
                        DataPoints.Add(FactDataPoint.Parse(factdata));
                    }
                }
            }
            ToReturn.DataPoints = DataPoints.ToArray();

            return ToReturn;
        }
    }
}