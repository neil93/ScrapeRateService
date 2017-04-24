using Common.Logging;

namespace ScrapeRateService.Strategy
{
    internal abstract class ScrapeStrategy
    {
        protected readonly ILog _log = LogManager.GetLogger(typeof(ScrapeStrategy));

        internal abstract string Execute();
    }
}
