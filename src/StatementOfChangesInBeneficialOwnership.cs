using System;
using System.Xml;

namespace SecuritiesExchangeCommission.Edgar
{
    public class StatementOfChangesInBeneficialOwnership //Form 4
    {
        //Document data
        public string SchemaVersion {get; set;}
        public DateTime PeriodOfReport {get; set;}

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

        public static StatementOfChangesInBeneficialOwnership ParseXml(string xml)
        {
            StatementOfChangesInBeneficialOwnership ToReturn = new StatementOfChangesInBeneficialOwnership();

            //Load the xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //Get the ownership document node that contains all data
            XmlNode doc_data = doc.SelectSingleNode("ownershipDocument");

            //Get schema version
            ToReturn.SchemaVersion = doc_data.SelectSingleNode("schemaVersion").InnerText;
            
            //Get period of report
            ToReturn.PeriodOfReport = DateTime.Parse(doc_data.SelectSingleNode("periodOfReport").InnerText);
            
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
            string owner_is_officer_val = node_reportingOwnerRelationship.SelectSingleNode("isOfficer").InnerText;
            if (owner_is_officer_val == "0")
            {
                ToReturn.OwnerIsOfficer = false;
            }
            else if (owner_is_officer_val == "1")
            {
                ToReturn.OwnerIsOfficer = true;
            }
            ToReturn.OwnerOfficerTitle = node_reportingOwnerRelationship.SelectSingleNode("officerTitle").InnerText;

            #endregion

            return ToReturn;
        }

    }
}