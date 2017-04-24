using ScrapeRateService.Model;
using System;

namespace ScrapeRateService.Strategy
{
    public class StrategyFactory
    {
        internal static ScrapeStrategy GetStrategy(ScrapeTypeConstant type)
        {
            switch (type)
            {
                case ScrapeTypeConstant.TaiwanBank:
                    return new StrategyScrapeTaiwanBank();

                case ScrapeTypeConstant.CTCB:
                    return new StrategyScrapeCTCB();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
