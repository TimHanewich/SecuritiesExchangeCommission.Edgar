using System;
using System.Xml;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SecuritiesExchangeCommission.Edgar
{

    /// <summary>
    /// SEC Form 3, 4, or 5, relating to beneficial ownership of an entity.
    /// </summary>
    public class StatementOfBeneficialOwnership
    {
        //Document data
        public string SchemaVersion {get; set;}
        public DateTime PeriodOfReport {get; set;}
        public string DocumentType {get; set;} //i.e. 3, 4, 5

        //About the issuer (the company)
        public string IssuerCik {get; set;}
        public string IssuerName {get; set;}
        public string IssuerTradingSymbol {get; set;}

        //Information about the reporting owner (the person)
        public string OwnerName {get; set;}
        public string OwnerCik {get; set;}
        public string OwnerStreet1 {get; set;}
        public string OwnerStreet2 {get; set;}
        public string OwnerCity {get; set;}
        public string OwnerStateCode {get; set;}
        public string OwnerZipCode {get; set;}
        public bool OwnerIsOfficer {get; set;}
        public string OwnerOfficerTitle {get; set;}

        //Non derivative table (transactions and holdings)
        public NonDerivativeTransaction[] NonDerivativeTransactions {get; set;}

        //Derivative transactions
        public DerivativeTransaction[] DerivativeTransactions {get; set;}

        public static StatementOfBeneficialOwnership ParseXml(string xml)
        {
            StatementOfBeneficialOwnership ToReturn = new StatementOfBeneficialOwnership();

            //Load the xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //Get the ownership document node that contains all data
            XmlNode doc_data = doc.SelectSingleNode("ownershipDocument");

            //Get schema version
            if (doc_data.SelectSingleNode("schemaVersion") != null)
            {
                ToReturn.SchemaVersion = doc_data.SelectSingleNode("schemaVersion").InnerText;
            }
            else
            {
                ToReturn.SchemaVersion = null;
            }
            
            //Get period of report
            ToReturn.PeriodOfReport = DateTime.Parse(doc_data.SelectSingleNode("periodOfReport").InnerText);
            
            //documentType
            XmlNode node_documentType = doc_data.SelectSingleNode("documentType");
            if (node_documentType != null)
            {
                ToReturn.DocumentType = node_documentType.InnerText.Trim();
            }
            else
            {
                ToReturn.DocumentType = null;
            }

            #region "Issuer"

            XmlNode issuer = doc_data.SelectSingleNode("issuer");
            
            //CIK
            ToReturn.IssuerCik = issuer.SelectSingleNode("issuerCik").InnerText;

            //Issuer name
            ToReturn.IssuerName = issuer.SelectSingleNode("issuerName").InnerText;

            //Issuer trading symbol
            ToReturn.IssuerTradingSymbol = issuer.SelectSingleNode("issuerTradingSymbol").InnerText;

            #endregion
            
            #region "Owner"

            //Get the reporting owner node
            XmlNode node_reportingowner = doc_data.SelectSingleNode("reportingOwner");
            
            //Reporting owner id data
            XmlNode node_reportingOwnerId = node_reportingowner.SelectSingleNode("reportingOwnerId");
            ToReturn.OwnerCik = node_reportingOwnerId.SelectSingleNode("rptOwnerCik").InnerText;
            ToReturn.OwnerName = node_reportingOwnerId.SelectSingleNode("rptOwnerName").InnerText;

            //Reporting owner address data
            XmlNode node_reportingOwnerAddress = node_reportingowner.SelectSingleNode("reportingOwnerAddress");
            ToReturn.OwnerStreet1 = node_reportingOwnerAddress.SelectSingleNode("rptOwnerStreet1").InnerText;
            ToReturn.OwnerStreet2 = node_reportingOwnerAddress.SelectSingleNode("rptOwnerStreet2").InnerText;
            ToReturn.OwnerCity = node_reportingOwnerAddress.SelectSingleNode("rptOwnerCity").InnerText;
            ToReturn.OwnerStateCode = node_reportingOwnerAddress.SelectSingleNode("rptOwnerState").InnerText;
            ToReturn.OwnerZipCode = node_reportingOwnerAddress.SelectSingleNode("rptOwnerZipCode").InnerText;

            //Owner relationship
            XmlNode node_reportingOwnerRelationship = node_reportingowner.SelectSingleNode("reportingOwnerRelationship");

            //Owner is officer (this is sometimes called "Director")
            XmlNode node_isOfficer = node_reportingOwnerRelationship.SelectSingleNode("isOfficer");
            XmlNode node_isDirector = node_reportingOwnerRelationship.SelectSingleNode("isDirector");
            if (node_isOfficer != null)
            {
                string owner_is_officer_val = node_reportingOwnerRelationship.SelectSingleNode("isOfficer").InnerText;
                if (owner_is_officer_val == "0")
                {
                    ToReturn.OwnerIsOfficer = false;
                }
                else if (owner_is_officer_val == "1")
                {
                    ToReturn.OwnerIsOfficer = true;
                }
            }
            else if (node_isDirector != null) //If the isOfficer node was null, check for the isDirector node instead.
            {
                string owner_is_officer_val = node_reportingOwnerRelationship.SelectSingleNode("isDirector").InnerText;
                if (owner_is_officer_val == "0")
                {
                    ToReturn.OwnerIsOfficer = false;
                }
                else if (owner_is_officer_val == "1")
                {
                    ToReturn.OwnerIsOfficer = true;
                }
            }
            else //If we couldn't find a relevant tag, just say they are not an officer
            {
                ToReturn.OwnerIsOfficer = false;
            }

            

            //Get the officer title (this will only be there if this person is an officer)
            if (ToReturn.OwnerIsOfficer)
            {
                XmlNode node_officerTitle = node_reportingOwnerRelationship.SelectSingleNode("officerTitle");
                if (node_officerTitle != null)
                {
                    ToReturn.OwnerOfficerTitle = node_reportingOwnerRelationship.SelectSingleNode("officerTitle").InnerText;
                }
                else
                {
                    ToReturn.OwnerOfficerTitle = null;
                }
            }
            else
            {
                ToReturn.OwnerOfficerTitle = null;
            }
            

            #endregion

            #region "Non Derivative table"

            XmlNode node_nonDerivativeTable = doc_data.SelectSingleNode("nonDerivativeTable");
            if (node_nonDerivativeTable != null)
            {
                List<NonDerivativeTransaction> transactions = new List<NonDerivativeTransaction>();
                foreach (XmlNode node_nonDerivativeEntry in node_nonDerivativeTable.ChildNodes)
                {
                    NonDerivativeTransaction ndt = new NonDerivativeTransaction();
                    ndt.LoadFromNode(node_nonDerivativeEntry);
                    transactions.Add(ndt);
                }
                ToReturn.NonDerivativeTransactions = transactions.ToArray();
            }

            #endregion

            #region "Derivative table"
            
            XmlNode node_derivativeTable = doc_data.SelectSingleNode("derivativeTable");
            if (node_derivativeTable != null)
            {
                List<DerivativeTransaction> DTs = new List<DerivativeTransaction>();
                foreach (XmlNode node_derivativeTransaction in node_derivativeTable.ChildNodes)
                {
                    DerivativeTransaction dt = new DerivativeTransaction();
                    dt.LoadFromNode(node_derivativeTransaction);
                    DTs.Add(dt);
                }
                ToReturn.DerivativeTransactions = DTs.ToArray();
            }

            #endregion

            return ToReturn;
        }

        public static async Task<StatementOfBeneficialOwnership> ParseXmlFromWebUrlAsync(string url)
        {
            string content = await SecRequestManager.Instance.SecGetAsync(url);
            StatementOfBeneficialOwnership ToReturn = StatementOfBeneficialOwnership.ParseXml(content);
            return ToReturn;
        }

        public static TransactionType TransactionTypeFromCode(string code)
        {
            List<KeyValuePair<string, TransactionType>> KVPs = new List<KeyValuePair<string, TransactionType>>();
            KVPs.Add(new KeyValuePair<string, TransactionType>("P", TransactionType.OpenMarketOrPrivatePurchase));
            KVPs.Add(new KeyValuePair<string, TransactionType>("S", TransactionType.OpenMarketOrPrivateSale));
            KVPs.Add(new KeyValuePair<string, TransactionType>("V", TransactionType.TransactionVoluntarilyReportedEarlierThanRequired));
            KVPs.Add(new KeyValuePair<string, TransactionType>("A", TransactionType.GrantOrAward));
            KVPs.Add(new KeyValuePair<string, TransactionType>("D", TransactionType.SaleBackToIssuer));
            KVPs.Add(new KeyValuePair<string, TransactionType>("F", TransactionType.PaymentOfExercisePriceOrTaxLiability));
            KVPs.Add(new KeyValuePair<string, TransactionType>("I", TransactionType.DiscretionaryTransaction));
            KVPs.Add(new KeyValuePair<string, TransactionType>("M", TransactionType.ExerciseOrConversionOfDerivativeSecurity));
            KVPs.Add(new KeyValuePair<string, TransactionType>("C", TransactionType.ConversionOfDerivativeSecurity));
            KVPs.Add(new KeyValuePair<string, TransactionType>("E", TransactionType.ExpirationOfShortDerivativePosition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("H", TransactionType.ExpirationOfLongDerivativePosition));
            KVPs.Add(new KeyValuePair<string, TransactionType>("O", TransactionType.ExerciseOfOutOfMoneyDerivative));
            KVPs.Add(new KeyValuePair<string, TransactionType>("X", TransactionType.ExerciseOfInMoneyDerivative));
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