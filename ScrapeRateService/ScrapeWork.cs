using Common.Logging;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeRateService
{
    public class ScrapeWork
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScrapeWork));

        private static readonly Lazy<ScrapeWork> LazyInstance = new Lazy<ScrapeWork>(() => new ScrapeWork());

        private ScrapeWork() { }

        public static ScrapeWork Instance { get { return LazyInstance.Value; } }

        public string Execute()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string url = "http://rate.bot.com.tw/xrt?Lang=zh-TW";
                var htmlWeb = new HtmlWeb();
                var doc = htmlWeb.Load(url);
                var nodes = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main/div[4]/table").InnerHtml;
                HtmlDocument hdc = new HtmlDocument();
                hdc.LoadHtml(nodes);

                var list = new Dictionary<string, int[]>();
                list.Add("美金", new int[] { 1, 3 });
                list.Add("日幣", new int[] { 8, 3 });
                list.Add("人民幣", new int[] { 19, 3 });
                list.Add("澳幣", new int[] { 4, 3 });
                list.Add("加拿大幣", new int[] { 5, 3 });

                sb.AppendLine("");
                sb.AppendLine($"台灣銀行現金匯率：{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
                log.Info($"台灣銀行現金匯率：{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
                foreach (var key in list)
                {
                    var cashSalePrice = hdc.DocumentNode.SelectSingleNode($"/tbody/tr[{key.Value[0]}]/td[{key.Value[1]}]").InnerText;
                    sb.AppendLine($"{key.Key}:{cashSalePrice}");
                    log.Info($"{key.Key}:{cashSalePrice}");
                }

                string desc = "其它匯率請參考：http://rate.bot.com.tw/xrt?Lang=zh-TW";
                string desc1 = "通報時間=>週一~週五(AM:09:00~PM:19:00)";

                sb.AppendLine(desc);
                sb.AppendLine(desc1);
                log.Info(desc + desc1);
                doc = null;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return string.Empty;
        }
    }
}
