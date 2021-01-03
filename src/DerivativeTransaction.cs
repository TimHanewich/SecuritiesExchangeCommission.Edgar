using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class DerivativeTransaction
    {
        public string DerivativeSecurityTitle {get; set;}
        public DateTime TransactionDate {get; set;}
        public TransactionType TransactionCode {get; set;}
        public uint Quantity {get; set;}
        public DateTime Excersisable {get; set;}
        public DateTime Expiration {get; set;}
        public string UnderlyingSecurityTitle {get; set;}
        public uint UnderlyingSecurityQuantity {get; set;}
        public float DerivativeSecurityPrice {get; set;}
        public uint DerivativeSecuritiesOwnedFollowingTransaction {get; set;}
        public OwnershipNature DirectOrIndirectOwnership {get; set;}
    }
}