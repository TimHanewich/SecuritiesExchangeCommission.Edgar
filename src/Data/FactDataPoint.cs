using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SecuritiesExchangeCommission.Edgar.Data
{
    public class FactDataPoint
    {
        public DateTime? Start { get; set; } //only used for period-based facts
        public DateTime End { get; set; }
        public float Value { get; set; }
        public int? FiscalYear { get; set; } //i.e. 2010
        public FiscalPeriod Period { get; set; } //"fp": i.e. "Q3" or "FY"
        public string FromForm { get; set; } //i.e. 10-Q
        public DateTime Filed { get; set; } //The date it was filed on

        public static FactDataPoint Parse(JObject jo)
        {
            FactDataPoint ToReturn = new FactDataPoint();

            JProperty prop_start = jo.Property("start");
            if (prop_start != null)
            {
                if (prop_start.Value.Type != JTokenType.Null)
                {
                    ToReturn.Start = DateTime.Parse(prop_start.Value.ToString());
                }
            }

            JProperty prop_end = jo.Property("end");
            if (prop_end != null)
            {
                ToReturn.End = DateTime.Parse(prop_end.Value.ToString());
            }

            //Get val
            JProperty prop_val = jo.Property("val");
            if (prop_val != null)
            {
                if (prop_val.Value.Type != JTokenType.Null)
                {
                    ToReturn.Value = Convert.ToSingle(prop_val.Value.ToString());
                }
            }

            //Fiscal Year
            JProperty prop_fy = jo.Property("fy");
            if (prop_fy != null)
            {
                if (prop_fy.Value.Type != JTokenType.Null)
                {
                    
                }
            }

            //FP (for period)
            JProperty prop_fp = jo.Property("fp");
            if (prop_fp != null)
            {
                if (prop_fp.Value.Type != JTokenType.Null)
                {
                    string fp = prop_fp.Value.ToString();
                    if (fp.ToLower() == "fy")
                    {
                        ToReturn.Period = FiscalPeriod.FiscalYear;
                    }
                    else if (fp.ToLower() == "q1")
                    {
                        ToReturn.Period = FiscalPeriod.Q1;
                    }
                    else if (fp.ToLower() == "q2")
                    {
                        ToReturn.Period = FiscalPeriod.Q2;
                    }
                    else if (fp.ToLower() == "q3")
                    {
                        ToReturn.Period = FiscalPeriod.Q3;
                    }
                    else if (fp.ToLower() == "q4")
                    {
                        ToReturn.Period = FiscalPeriod.Q4;
                    }
                }
            }

            //form
            if (jo.TryGetValue("form", out JToken val_form)) { ToReturn.FromForm = val_form.ToString(); }

            //filed
            if (jo.TryGetValue("filed", out JToken val_filed)) { ToReturn.Filed = DateTime.Parse(val_filed.ToString()); }

            return ToReturn;
        }
    }
}