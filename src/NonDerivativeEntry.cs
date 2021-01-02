using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class NonDerivativeEntry
    {
        public string SecurityTitle {get; set;}
        public uint SharesOwnedFollowingTransaction {get; set;} //Post transaction amounts
        public OwnershipNature DirectOrIndirectOwnership {get; set;} //Direct or indirect ownership
    }
}