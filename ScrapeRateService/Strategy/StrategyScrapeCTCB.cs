using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapeRateService.Strategy
{
    internal class StrategyScrapeCTCB : ScrapeStrategy
    {
        internal override string Execute()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string url = "https://www.ctbcbank.com/CTCBPortalWeb/toPage?id=TW_RB_CM_ebank_018001";
                var htmlWeb = new HtmlWeb();
                var doc = htmlWeb.Load(url);
                var nodes = doc.DocumentNode.SelectSingleNode("//*[@id='mainTable']").InnerHtml;
                HtmlDocument hdc = new HtmlDocument();
                hdc.LoadHtml(nodes);
                ////*[@id="mainTable"]
                var list = new Dictionary<string, int[]>();
                list.Add("美金", new int[] { 2, 3 });
                list.Add("日幣", new int[] { 3, 3 });
                list.Add("人民幣", new int[] { 21, 3 });
                list.Add("澳幣", new int[] { 10, 3 });
                list.Add("加拿大幣", new int[] { 11, 3 });
                list.Add("紐元", new int[] { 13, 3 });

                sb.AppendLine("");
                sb.AppendLine($"中國信託現金匯率：{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
                foreach (var key in list)
                {
                    var price = 0.0;
                    var cashSalePrice =double.TryParse( hdc.DocumentNode.SelectSingleNode($"/tr[{key.Value[0]}]/td[{key.Value[1]}]").InnerText,out price );
                    sb.AppendLine($"{key.Key}:{price}");
                }

                sb.AppendLine($"其它匯率請參考：{url}\r\n通報時間=>週一~週五(AM:09:00~PM:19:00)");

                doc = null;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                base._log.Error(ex);
            }
            return string.Empty;
        }
    }
}
