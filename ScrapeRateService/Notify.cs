using Common.Logging;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Configuration;
using System.Text;

namespace ScrapeRateService
{
    public class Notify
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Notify));

        private static readonly Lazy<Notify> LazyInstance = new Lazy<Notify>(() => new Notify());

        private static string token = ConfigurationManager.AppSettings["LineToken"];

        private static string lineServiceUri = ConfigurationManager.AppSettings["LineServiceUri"];

        private Notify() { }

        public static Notify Instance { get { return LazyInstance.Value; } }

        public void PostMessage(string message)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Authorization", token);
                    NameValueCollection nc = new NameValueCollection();
                    nc["message"] = message;

                    byte[] bResult = client.UploadValues(lineServiceUri, nc);
                    string resultXML = Encoding.UTF8.GetString(bResult);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
