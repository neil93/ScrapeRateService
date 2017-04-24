using Common.Logging;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Configuration;
using System.Text;

namespace ScrapeRateService.BLL
{
    public class NotifyUser
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifyUser));

        private static readonly Lazy<NotifyUser> LazyInstance = new Lazy<NotifyUser>(() => new NotifyUser());

        private static string token = ConfigurationManager.AppSettings["LineToken"];

        private static string lineServiceUri = ConfigurationManager.AppSettings["LineServiceUri"];

        private NotifyUser() { }

        public static NotifyUser Instance { get { return LazyInstance.Value; } }

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
