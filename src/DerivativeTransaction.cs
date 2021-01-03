using System;
using System.Xml;

namespace SecuritiesExchangeCommission.Edgar
{
    public class DerivativeTransaction
    {
        public string DerivativeSecurityTitle {get; set;}
        public DateTime TransactionDate {get; set;}
        public TransactionType TransactionCode {get; set;}
        public float Quantity {get; set;}
        public AcquiredDisposed AcquiredOrDisposed {get; set;}
        public DateTime? Excersisable {get; set;}
        public DateTime? Expiration {get; set;}
        public string UnderlyingSecurityTitle {get; set;}
        public float UnderlyingSecurityShares {get; set;}
        public float DerivativeSecurityPricePerShare {get; set;}
        public float DerivativeSecuritiesOwnedFollowingTransaction {get; set;}
        public OwnershipNature DirectOrIndirectOwnership {get; set;}

        public void LoadFromNode(XmlNode node)
        {
            //Security title
            XmlNode node_securityTitle = node.SelectSingleNode("securityTitle");
            if (node_securityTitle != null)
            {
                XmlNode node_value = node_securityTitle.SelectSingleNode("value");
                if (node_value != null)
                {
                    DerivativeSecurityTitle = node_value.InnerText;
                }
            }

            //Transaction Date
            XmlNode node_transactionDate = node.SelectSingleNode("transactionDate");
            if (node_transactionDate != null)
            {
                XmlNode node_value = node_transactionDate.SelectSingleNode("value");
                if (node_value != null)
                {
                    TransactionDate = DateTime.Parse(node_value.InnerText);
                }
            }

            //Transaction Code
            XmlNode node_transactionCoding = node.SelectSingleNode("transactionCoding");
            if (node_transactionCoding != null)
            {
                XmlNode node_transactionCode = node_transactionCoding.SelectSingleNode("transactionCode");
                if (node_transactionCode != null)
                {
                    string tc = node_transactionCode.InnerText;
                    TransactionCode = StatementOfChangesInBeneficialOwnership.TransactionTypeFromCode(tc);
                }
            }

            //transactionAmounts node (# of shares, acquired or disposed of)
            XmlNode node_transactionAmounts = node.SelectSingleNode("transactionAmounts");
            if (node_transactionAmounts != null)
            {
                //# of shares
                XmlNode node_transactionShares = node_transactionAmounts.SelectSingleNode("transactionShares");
                if (node_transactionShares != null)
                {
                    XmlNode node_value = node_transactionShares.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        string quantity_str = node_value.InnerText;
                        Quantity = Convert.ToSingle(quantity_str);
                    }
                }

                //Acquired or disposed of
                XmlNode node_transactionAcquiredDisposedCode = node_transactionAmounts.SelectSingleNode("transactionAcquiredDisposedCode");
                if (node_transactionAcquiredDisposedCode != null)
                {
                    XmlNode node_value = node_transactionAcquiredDisposedCode.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        string adcode = node_value.InnerText.ToLower();
                        if (adcode == "a")
                        {
                            AcquiredOrDisposed = AcquiredDisposed.Acquired;
                        }
                        else if (adcode == "d")
                        {
                            AcquiredOrDisposed = AcquiredDisposed.Disposed;
                        }
                    }
                }
            
                //price per share (derivative price)
                XmlNode node_transactionPricePerShare = node_transactionAmounts.SelectSingleNode("transactionPricePerShare");
                if (node_transactionPricePerShare != null)
                {
                    XmlNode node_value = node_transactionPricePerShare.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        DerivativeSecurityPricePerShare = Convert.ToSingle(node_value.InnerText);
                    }
                }
            }

            //Date excercisable, expiration date
            XmlNode node_excersizeDate = node.SelectSingleNode("exerciseDate");
            if (node_excersizeDate != null)
            {
                //There will typically be either a value with a date or a footnote (footnote is more common)
                XmlNode node_value = node_excersizeDate.SelectSingleNode("value");
                if (node_value != null)
                {
                    Excersisable = DateTime.Parse(node_value.InnerText);
                }
                else
                {
                    Excersisable = null;
                }
            }
            else
            {
                Excersisable = null;
            }

            //Expiration date
            XmlNode node_expirationDate = node.SelectSingleNode("expirationDate");
            if (node_expirationDate != null)
            {
                //There will typically be either a value with a date or a footnote (footnote is more common)
                XmlNode node_value = node_expirationDate.SelectSingleNode("value");
                if (node_value != null)
                {
                    Expiration = DateTime.Parse(node_value.InnerText);
                }
                else
                {
                    Expiration = null;
                }
            }
            else
            {
                Expiration = null;
            }
            
            //Underlying secuity section
            XmlNode node_underlyingSecurity = node.SelectSingleNode("underlyingSecurity");
            if (node_underlyingSecurity != null)
            {
                //Underlying security title
                XmlNode node_underlyingSecurityTitle = node_underlyingSecurity.SelectSingleNode("underlyingSecurityTitle");
                if (node_underlyingSecurityTitle != null)
                {
                    XmlNode node_value = node_underlyingSecurityTitle.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        UnderlyingSecurityTitle = node_value.InnerText;
                    }
                }

                //Underlying security shares quantity
                XmlNode node_underlyingSecurityShares = node_underlyingSecurity.SelectSingleNode("underlyingSecurityShares");
                if (node_underlyingSecurityShares != null)
                {
                    XmlNode node_value = node_underlyingSecurityShares.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        UnderlyingSecurityShares = Convert.ToSingle(node_value.InnerText);
                    }
                }
            }

            //Shares owned following transaction
            XmlNode node_postTransactionAmonts = node.SelectSingleNode("postTransactionAmounts");
            if (node_postTransactionAmonts != null)
            {
                XmlNode node_sharesOwnedFollowingTransaction = node_postTransactionAmonts.SelectSingleNode("sharesOwnedFollowingTransaction");
                if (node_sharesOwnedFollowingTransaction != null)
                {
                    XmlNode node_value = node_sharesOwnedFollowingTransaction.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        DerivativeSecuritiesOwnedFollowingTransaction = Convert.ToSingle(node_value.InnerText);
                    }
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
                        string diocode = node_value.InnerText.ToLower();
                        if (diocode == "d")
                        {
                            DirectOrIndirectOwnership = OwnershipNature.Direct;
                        }
                        else
                        {
                            DirectOrIndirectOwnership = OwnershipNature.Indirect;
                        }
                    }
                }
            }
        }
    }
}