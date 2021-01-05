using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class SecurityTransaction : SecurityEntry
    {
        public DateTime TransactionDate {get; set;}
        public TransactionType TransactionCode {get; set;}
        public float Quantity {get; set;}
        public float PricePerSecurity {get; set;}
        public AcquiredDisposed AcquiredOrDisposed {get; set;}
    }
}