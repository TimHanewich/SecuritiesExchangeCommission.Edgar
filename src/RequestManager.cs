using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public class RequestManager
    {
        //Modifiable propertie
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string Email { get; set; }

        //Create version of it
        private static readonly RequestManager _instance = new RequestManager();

        //Reference
        public static RequestManager Instance
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