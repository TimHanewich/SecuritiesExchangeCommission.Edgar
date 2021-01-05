using System;
using System.Xml;
using System.Collections.Generic;

namespace SecuritiesExchangeCommission.Edgar
{
    public class NonDerivativeTransaction : SecurityTransaction
    {
        public override void LoadFromNode(XmlNode node)
        {
            //Load the data from the sub-class
            base.LoadFromNode(node);

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
                        Quantity = Convert.ToSingle(node_value.InnerText);
                    }
                }

                //Transaction price per share
                XmlNode node_transactionPricePerShare = node_transactionAmounts.SelectSingleNode("transactionPricePerShare");
                if (node_transactionPricePerShare != null)
                {
                    XmlNode node_value = node_transactionPricePerShare.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        PricePerSecurity = Convert.ToSingle(node_value.InnerText);
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

        
    }
}