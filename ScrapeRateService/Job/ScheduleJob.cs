using Quartz;
using Common.Logging;
using ScrapeRateService.BLL;
using ScrapeRateService.Strategy;

namespace ScrapeRateService.Job
{
    public class ScheduleJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScheduleJob));

        public void Execute(IJobExecutionContext context)
        {
            var scrapeWork = StrategyFactory.GetStrategy(Model.ScrapeTypeConstant.TaiwanBank);
            var result = scrapeWork.Execute();

            if (!string.IsNullOrEmpty(result))
                log.Info(result);
                NotifyUser.Instance.PostMessage(result);
        }
    }
}
