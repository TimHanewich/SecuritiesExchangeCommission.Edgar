using System;
using System.Xml;

namespace SecuritiesExchangeCommission.Edgar
{
    public class SecurityTransaction
    {
        public string SecurityTitle {get; set;} //Will be there whether this is a transaction or holding
        public float SecuritiesOwnedFollowingTransaction {get; set;} //Will be there whether this is a transaction or holding
        public OwnershipNature DirectOrIndirectOwnership {get; set;} //Will be there whether this is a transaction or holding
        public DateTime? TransactionDate {get; set;} //Will only be populated if it is a transaction. Null if a holding
        public TransactionType? TransactionCode {get; set;}
        public float? TransactionQuantity {get; set;} 
        public float? TransactionPricePerSecurity {get; set;}
        public AcquiredDisposed? AcquiredOrDisposed {get; set;}

        public virtual void LoadFromNode(XmlNode node)
        {
            //Security title
            XmlNode node_securityTitle = node.SelectSingleNode("securityTitle");
            if (node_securityTitle != null)
            {
                XmlNode node_value = node_securityTitle.SelectSingleNode("value");
                SecurityTitle = node_value.InnerText;
            }

            //Shares Owned following transaction
            XmlNode node_postTransactionAmounts = node.SelectSingleNode("postTransactionAmounts");
            if (node_postTransactionAmounts != null)
            {
                //Shares owned following transaction
                XmlNode node_sharesOwnedFollowingTransaction = node_postTransactionAmounts.SelectSingleNode("sharesOwnedFollowingTransaction");
                if (node_sharesOwnedFollowingTransaction != null)
                {
                    XmlNode node_value = node_sharesOwnedFollowingTransaction.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        string node_val_str = node_value.InnerText;
                        SecuritiesOwnedFollowingTransaction = Convert.ToSingle(node_val_str);
                    }
                }

                //Direct or indirect ownership
                XmlNode node_ownershipNature = node.SelectSingleNode("ownershipNature");
                if (node_ownershipNature != null)
                {
                    XmlNode node_directOrIndirectOwnership = node_ownershipNature.SelectSingleNode("directOrIndirectOwnership");
                    if (node_directOrIndirectOwnership != null)
                    {
                        XmlNode node_value = node_directOrIndirectOwnership.SelectSingleNode("value");
                        if (node_value != null)
                        {
                            string val = node_value.InnerText;
                            if (val.ToLower() == "d")
                            {
                                DirectOrIndirectOwnership = OwnershipNature.Direct;                
                            }
                            else if (val.ToLower() == "i")
                            {
                                DirectOrIndirectOwnership = OwnershipNature.Indirect;
                            }
                        }
                    }
                }

            }
        
            //Transaction date
            XmlNode node_transactionDate = node.SelectSingleNode("transactionDate");
            if (node_transactionDate != null)
            {
                XmlNode node_value = node_transactionDate.SelectSingleNode("value");
                if (node_value != null)
                {
                    TransactionDate = DateTime.Parse(node_value.InnerText);
                }
            }

            //Transaction code
            XmlNode node_transactionCoding = node.SelectSingleNode("transactionCoding");
            if (node_transactionCoding != null)
            {
                XmlNode node_transactionCode = node_transactionCoding.SelectSingleNode("transactionCode");
                if (node_transactionCode != null)
                {
                    string tc = node_transactionCode.InnerText.ToLower();
                    TransactionCode = StatementOfChangesInBeneficialOwnership.TransactionTypeFromCode(tc);
                }
            }

            //Transaction Amounts section (# of shares, price per share, acquired or disposed)
            XmlNode node_transactionAmounts = node.SelectSingleNode("transactionAmounts");
            if (node_transactionAmounts != null)
            {
                //Transaction Shares Quanity
                XmlNode node_transactionShares = node_transactionAmounts.SelectSingleNode("transactionShares");
                if (node_transactionShares != null)
                {
                    XmlNode node_value = node_transactionShares.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        TransactionQuantity = Convert.ToSingle(node_value.InnerText);
                    }
                }

                //Transaction price per share
                XmlNode node_transactionPricePerShare = node_transactionAmounts.SelectSingleNode("transactionPricePerShare");
                if (node_transactionPricePerShare != null)
                {
                    XmlNode node_value = node_transactionPricePerShare.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        TransactionPricePerSecurity = Convert.ToSingle(node_value.InnerText);
                    }
                }

                //Acquired or disposed?
                XmlNode node_transactionAcquiredDisposedCode = node_transactionAmounts.SelectSingleNode("transactionAcquiredDisposedCode");
                if (node_transactionAcquiredDisposedCode != null)
                {
                    XmlNode node_value = node_transactionAcquiredDisposedCode.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        string ad_code = node_value.InnerText.ToLower();
                        if (ad_code == "a")
                        {
                            AcquiredOrDisposed = AcquiredDisposed.Acquired;
                        }
                        else if (ad_code == "d")
                        {
                            AcquiredOrDisposed = AcquiredDisposed.Disposed;
                        }
                    }
                }
            }
        }
    
        public bool IsTransaction()
        {
            //If any of the transaction values are NOT null, it is a transaction
            
            bool ToReturn = false;

            if (TransactionDate.HasValue)
            {
                ToReturn = true;
            }
            else if (TransactionCode.HasValue)
            {
                ToReturn = true;
            }
            else if (TransactionQuantity.HasValue)
            {
                ToReturn = true;
            }
            else if (TransactionPricePerSecurity.HasValue)
            {
                ToReturn = true;
            }
            else if (AcquiredOrDisposed.HasValue)
            {
                ToReturn = true;
            }

            return ToReturn;
        }
    
        public bool IsHolding()
        {
            return !IsTransaction();
        }
    }
}