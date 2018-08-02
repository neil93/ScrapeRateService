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

namespace ScrapeRateService
{

    partial class MainService :ServiceBase
    {
        private IScheduler _schedular = null; // Quartz排程器

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

           // IScheduler _scheduler = new StdSchedulerFactory().GetScheduler();

            ISchedulerFactory sf = new Quartz.Impl.StdSchedulerFactory();
            _schedular = sf.GetScheduler();

            //_schedular.Start(); // 啟動Quartz排程器



            _schedular.Start();



            //# 建立固定排程工作１,原則上一個Job呼應一個Trigger。
            IJobDetail job1 = JobBuilder.Create<ScrapeRateService.Job.ScheduleJob>()
                                 .WithIdentity("job1")
                                 .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                                 .WithIdentity("trigger1")
                                 .WithCronSchedule("0 0 9-19 ? * MON-FRI") // 每5秒觸發一次。
                                 .Build();

            _schedular.ScheduleJob(job1, trigger1);

#if DEBUG
            var scrapeWork = Strategy.StrategyFactory.GetStrategy(Model.ScrapeTypeConstant.TaiwanBank);
            var result = scrapeWork.Execute();
            _log.Info(result);



#endif
            shutdownEvent = new ManualResetEvent(false);

            //用於多個service
            //_services.Add(new RegularlyService());
            var ts = new ThreadStart(serviceStart);
            _thread = new Thread(ts);
            _thread.Start();
            base.OnStart(args);
        }


        protected static  NameValueCollection QuartzConfig()
        {
            NameValueCollection quartzProps = new NameValueCollection();

            quartzProps.Add("quartz.scheduler.instanceName", "MyScheduler");
            quartzProps.Add("quartz.scheduler.instanceId", "AUTO");
            quartzProps.Add("quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz");
            quartzProps.Add("quartz.threadPool.threadCount", "10");
            quartzProps.Add("quartz.threadPool.threadPriority", "Normal");

            quartzProps.Add("quartz.scheduler.idleWaitTime", "5000");

            quartzProps.Add("quartz.jobStore.misfireThreshold", "60000");
            quartzProps.Add("quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz");
            quartzProps.Add("quartz.jobStore.tablePrefix", "QRTZ_");
            quartzProps.Add("quartz.jobStore.clustered", "false");
            quartzProps.Add("quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz");
            quartzProps.Add("quartz.jobStore.lockHandler.type", "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz");
            quartzProps.Add("quartz.jobStore.useProperties", "true");

            //quartzProps.Add("quartz.jobStore.dataSource", "default");
            //quartzProps.Add("quartz.dataSource.default.connectionString", @"Data Source=YourPC\sqlexpress;Initial Catalog=Quartz;Integrated Security=True");
            //quartzProps.Add("quartz.dataSource.default.provider", "SqlServer-20");

            return quartzProps;
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
