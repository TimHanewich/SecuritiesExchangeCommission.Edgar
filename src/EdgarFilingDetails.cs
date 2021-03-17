using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class EdgarFilingDetails
    {
        public long AccessionNumberP1 {get; set;}
        public int AccessionNumberP2 {get; set;}
        public int AccessionNumberP3 {get; set;}
        public string Form {get; set;}
        public DateTime FilingDate {get; set;}
        public DateTime PeriodOfReport {get; set;}
        public DateTime Accepted {get; set;}
        public FilingDocument[] DocumentFormatFiles {get; set;}
        public FilingDocument[] DataFiles {get; set;}
        public string EntityName {get; set;}
        public long EntityCik {get; set;}
    }
}