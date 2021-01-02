using System;
using System.Xml;

namespace SecuritiesExchangeCommission.Edgar
{
    public class NonDerivativeEntry
    {
        public string SecurityTitle {get; set;}
        public uint SharesOwnedFollowingTransaction {get; set;} //Post transaction amounts
        public OwnershipNature DirectOrIndirectOwnership {get; set;} //Direct or indirect ownership

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
                        SharesOwnedFollowingTransaction = Convert.ToUInt32(node_value.InnerText);
                    }
                }

                //Direct or indirect ownership
                XmlNode node_ownershipNature = node_postTransactionAmounts.SelectSingleNode("ownershipNature");
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

        }
    }
}