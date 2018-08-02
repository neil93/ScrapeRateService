using Common.Logging;
using Quartz;
using Quartz.Impl;
using ScrapeRateService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using ScrapeRateService.BLL;

namespace ScrapeRateService
{

    partial class MainService :ServiceBase
    {
        private ManualResetEvent shutdownEvent;
        private readonly IList<IService> _services = new List<IService>();
        private readonly ILog _log = LogManager.GetLogger(typeof(MainService));
        public MainService()
        {
            InitializeComponent();
        }

        internal void Start(string[] args)
        {
            OnStart(args);
        }
        private Thread _thread;
        protected override void OnStart(string[] args)
        {
            
            _log.Info("Service turned on.");

            //啟動quartz 排程
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler _schedular = sf.GetScheduler();
            _schedular.Start();

            //# 建立固定排程工作１,原則上一個Job呼應一個Trigger。
            IJobDetail job = JobBuilder.Create<Job.ScheduleJob>()
                                 .WithIdentity("job1", "group1")
                                 .Build();

            ITrigger trigger = TriggerBuilder.Create()
                                  .WithIdentity("trigger1", "group1")
                                  .WithCronSchedule("0 0 9-19 ? * MON-FRI") 
                                  //.WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())// 測試用每5秒觸發一次。
                                 .Build();

            _schedular.ScheduleJob(job, trigger);


#if DEBUG
            var scrapeWork = Strategy.StrategyFactory.GetStrategy(Model.ScrapeTypeConstant.TaiwanBank);
            var result = scrapeWork.Execute();

            if (!string.IsNullOrEmpty(result))
            {
                NotifyUser.Instance.PostMessage(result);
                _log.Info(result);
            }
#endif
            shutdownEvent = new ManualResetEvent(false);

            //用於多個service
            //_services.Add(new RegularlyService());
            var ts = new ThreadStart(serviceStart);
            _thread = new Thread(ts);
            _thread.Start();
            base.OnStart(args);
        }

       
        private void serviceStart()
        {
            foreach (var scrapingService in _services)
                scrapingService.Start();

            shutdownEvent.WaitOne();
            OnStop();
        }
        protected override void OnStop()
        {
            // signal the event to shutdown
            shutdownEvent.Set();
            _log.Info("Service shutting down.");
            foreach (var scrapingService in _services)
                scrapingService.Shutdown();

            // wait for the thread to stop giving it 10 seconds
            _thread.Join(10000);
            // call the base class 
            base.OnStop();
            _log.Info("Shutdown complete.");
        }

       
    }
}
