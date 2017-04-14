using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeRateService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            MainService _service = new MainService();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("ScrapeRateService is going to start as command mode.");
                _service.Start(args);
                commandMode();
            }
            else
            {
                ServiceBase.Run(_service);
            }
        }

        static void commandMode()
        {
            Console.Write("press 'q' for exit the program:");
            string input = Console.ReadLine();
            if (input != "q")
                commandMode();
        }
    }
}
