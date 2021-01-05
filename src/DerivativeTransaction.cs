using System;
using System.Xml;

namespace SecuritiesExchangeCommission.Edgar
{
    public class DerivativeTransaction : SecurityTransaction
    {
        public DateTime? Excersisable {get; set;} //This should technically always be here but sometimes it is put in the footnotes as an explanation, not a date time value. So it is null if it isn't directly put in as a datetime value.
        public DateTime? Expiration {get; set;} //This should technically always be here but sometimes it is put in the footnotes as an explanation, not a date time value. So it is null if it isn't directly put in as a datetime value.
        public string UnderlyingSecurityTitle {get; set;} //Will be there whether regardless of it being a transaction or holding
        public float UnderlyingSecurityQuantity {get; set;} //Will be there whether regardless of it being a transaction or holding

        public override void LoadFromNode(XmlNode node)
        {
            //Load the base
            base.LoadFromNode(node);

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
                        UnderlyingSecurityQuantity = Convert.ToSingle(node_value.InnerText);
                    }
                }
            }
        
        }
    }
}