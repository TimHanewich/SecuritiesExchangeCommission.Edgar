using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class FilingDocument
    {
        public int Sequence {get; set;}
        public string Description {get; set;}
        public string DocumentName {get; set;}
        public string Url {get; set;}
        public string DocumentType {get; set;}
        public int Size {get; set;}
    }
}