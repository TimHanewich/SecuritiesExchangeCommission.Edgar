using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarLatestFilingResult : EdgarFiling
    {
        public string EntityTitle {get; set;}
        public long EntityCik {get; set;}
    }
}