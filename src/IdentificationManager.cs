using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class IdentificationManager
    {
        //Modifiable propertie
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string Email { get; set; }

        //Create version of it
        private static readonly IdentificationManager _instance = new IdentificationManager();

        //Reference
        public static IdentificationManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public string ToUserAgent()
        {
            return AppName + "/" + AppVersion + " (" + Email + ")";
        }
        
    }
}