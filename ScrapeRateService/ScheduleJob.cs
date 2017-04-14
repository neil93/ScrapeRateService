using Quartz;
using Common.Logging;

namespace ScrapeRateService
{
    public class ScheduleJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScheduleJob));

        public void Execute(IJobExecutionContext context)
        {
            var result = ScrapeWork.Instance.Execute();
            if (!string.IsNullOrEmpty(result))
                Notify.Instance.PostMessage(result);
        }
    }
}
