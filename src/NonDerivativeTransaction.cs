using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class NonDerivativeTransaction : NonDerivativeEntry
    {
        public DateTime TransactionDate {get; set;}
        public TransactionType TransactionCode {get; set;}
        public uint TransactionShares {get; set;}
        public float PricePerShare {get; set;}
        public AcquiredDisposed AcquiredOrDisposed {get; set;}
    }
}