using System;
using System.Xml;
using System.Collections.Generic;

namespace SecuritiesExchangeCommission.Edgar
{
    public class NonDerivativeTransaction : NonDerivativeEntry
    {
        public DateTime TransactionDate {get; set;}
        public TransactionType TransactionCode {get; set;}
        public uint TransactionShares {get; set;}
        public float PricePerShare {get; set;}
        public AcquiredDisposed AcquiredOrDisposed {get; set;}

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
                    TransactionCode = TransactionTypeFromCode(tc);
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
                        TransactionShares = Convert.ToUInt32(node_value.InnerText);
                    }
                }

                //Transaction price per share
                XmlNode node_transactionPricePerShare = node_transactionAmounts.SelectSingleNode("transactionPricePerShare");
                if (node_transactionPricePerShare != null)
                {
                    XmlNode node_value = node_transactionPricePerShare.SelectSingleNode("value");
                    if (node_value != null)
                    {
                        PricePerShare = Convert.ToSingle(node_value.InnerText);
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

        private TransactionType TransactionTypeFromCode(string code)
        {
            List<KeyValuePair<string, TransactionType>> KVPs = new List<KeyValuePair<string, TransactionType>>();
            KVPs.Add(new KeyValuePair<string, TransactionType>("P", TransactionType.OpenMarketOrPrivatePurchase));
            KVPs.Add(new KeyValuePair<string, TransactionType>("S", TransactionType.OpenMarketOrPrivateSale));
            KVPs.Add(new KeyValuePair<string, TransactionType>("V", TransactionType.TransactionVoluntarilyReportedEarlierThanRequired));
            KVPs.Add(new KeyValuePair<string, TransactionType>("A", TransactionType.GrantOrAward));
            KVPs.Add(new KeyValuePair<string, TransactionType>("D", TransactionType.SaleBackToIssuer));
            KVPs.Add(new KeyValuePair<string, TransactionType>("F", TransactionType.PaymentOfExcersizePriceOrTaxLiability));
            KVPs.Add(new KeyValuePair<string, TransactionType>("I", TransactionType.DiscretionaryTransaction));
            KVPs.Add(new KeyValuePair<string, TransactionType>("M", TransactionType.ExcersizeOrConversionOfDerivativeSecurity));
            KVPs.Add(new KeyValuePair<string, TransactionType>("C", TransactionType.ConversionOfDerivativeSecurity));
            KVPs.Add(new KeyValuePair<string, TransactionType>("E", TransactionType.ExpirationOfShortDerivativePosition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("H", TransactionType.ExpirationOfLongDerivativePosition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("O", TransactionType.ExcersizeOfOutOfMoneyDerivative));
            KVPs.Add(new KeyValuePair<string, TransactionType>("X", TransactionType.ExcersizeOfInMoneyDerivative));
            KVPs.Add(new KeyValuePair<string, TransactionType>("G", TransactionType.BonaFideGift));
            KVPs.Add(new KeyValuePair<string, TransactionType>("L", TransactionType.SmallAcquisition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("W", TransactionType.AcquisitionOrDispositionByWillOrLaws));
            KVPs.Add(new KeyValuePair<string, TransactionType>("Z", TransactionType.DepositIntoOrWithdrawalFromVotingTrust));
            KVPs.Add(new KeyValuePair<string, TransactionType>("J", TransactionType.OtherAcquisitionOrDisposition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("K", TransactionType.TransactionInEquitySwap));
            KVPs.Add(new KeyValuePair<string, TransactionType>("U", TransactionType.DispositionDueToTenderOfShares));
        
            foreach (KeyValuePair<string, TransactionType> kvp in KVPs)
            {
                if (kvp.Key == code.ToUpper())
                {
                    return kvp.Value;
                }
            }

            throw new Exception("Unable to find transaction type for code '" + code + "'");
        }

    }
}