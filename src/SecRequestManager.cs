using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace SecuritiesExchangeCommission.Edgar
{
    public class SecRequestManager
    {
        //Instance of self
        public static SecRequestManager Instance = new SecRequestManager();

        //Events
        public event Notification StatusChanged; //Status was updated
        public event Action Throttled; //We were throttled by the SEC

        public TimeSpan RequestDelay {get; set;} //The time that will be waited before all requests
        public TimeSpan TimeoutDelay {get; set;} //The time that will be waited after we get a throttling warning
        public ushort MaxTryCount {get; set;} //The maximum number of trys that will occur before giving up (if it keeps getting throttle warnings)

        public SecRequestManager()
        {
            RequestDelay = new TimeSpan(0, 0, 0, 0, 250); //250 milliseconds (quarter of a second)
            TimeoutDelay = new TimeSpan(0, 0, 2); //2 seconds
            MaxTryCount = 10;
        }

        public SecRequestManager(TimeSpan request_delay, TimeSpan timeout_delay, ushort max_trys)
        {
            RequestDelay = request_delay;
            TimeoutDelay = timeout_delay;
            MaxTryCount = max_trys;
        }



        public async Task<string> SecGetAsync(string url)
        {
            HttpResponseMessage resp = await SecRequestAsync(url);
            string content = await resp.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<Stream> SecGetStreamAsync(string url)
        {
            HttpResponseMessage resp = await SecRequestAsync(url);
            Stream ToReturn = await resp.Content.ReadAsStreamAsync();
            return ToReturn;
        }

        //The above two methods use this
        private async Task<HttpResponseMessage> SecRequestAsync(string url)
        {
            
            HttpResponseMessage ToReturn = null;

            //Setup
            HttpClient hc = new HttpClient();
            int havetried = 0;

            while (ToReturn == null && havetried < MaxTryCount)
            {
                //Prepare the request
                TryUpdateStatus("Preparing request...");
                HttpRequestMessage req = PrepareHttpRequestMessage();
                req.RequestUri = new Uri(url);
                req.Method = HttpMethod.Get;

                //Take the request delay timeout first
                TryUpdateStatus("Taking request delay...");
                await Task.Delay(RequestDelay);
                
                //Make the call
                TryUpdateStatus("Attempting call...");
                HttpResponseMessage resp = await hc.SendAsync(req);
                TryUpdateStatus("Response received from the SEC server with code '" + resp.StatusCode.ToString() + "'");
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    ToReturn = resp;
                }
                else if (resp.StatusCode == HttpStatusCode.Forbidden) //Code 403 (throttled)
                {
                    if (Throttled != null) //Raise the throttled event
                    {
                        Throttled.Invoke();
                    }
                    TryUpdateStatus("Request was denied due to throttling. Waiting for timeout...");
                    await Task.Delay(TimeoutDelay);
                    TryUpdateStatus("Try count will be incremented and will try again.");
                }
                else if (resp.StatusCode == HttpStatusCode.ServiceUnavailable) //Service unavailable. For example, this will happen in this filing: https://www.sec.gov/Archives/edgar/data/1300695/000141588922008015/0001415889-22-008015-index.htm
                {
                    TryUpdateStatus("The SEC server indicated '503, Service Unavailable'.");
                    await Task.Delay(TimeoutDelay);
                    TryUpdateStatus("Try count will be incremented and will try again.");
                }
                else
                {
                    TryUpdateStatus("An unexpected response code was returned from the SEC: " + resp.StatusCode.ToString() + "(" + Convert.ToInt32(resp.StatusCode).ToString() + ")");
                    await Task.Delay(TimeoutDelay);
                    TryUpdateStatus("Try count will be incremented and will try again.");
                }

                //Increment the havetried count
                havetried = havetried + 1;
            }

            //If the have tried is what caused it (it is over the limit), throw an exception
            if (havetried >= MaxTryCount)
            {
                throw new Exception("Unable to get data for URL '" + url + "'. Surpassed maximum try count of " + MaxTryCount.ToString());
            }

            return ToReturn;
        }




        private void TryUpdateStatus(string msg)
        {
            if (StatusChanged != null)
            {
                StatusChanged.Invoke(msg);
            }
        }
    
        public HttpRequestMessage PrepareHttpRequestMessage()
        {
            HttpRequestMessage req = new HttpRequestMessage();
            req.Method = HttpMethod.Get;
            req.Headers.Add("User-Agent", IdentificationManager.Instance.ToUserAgent());
            
            req.Headers.Add("Accept", "*/*");
            return req;
        }
    }
}