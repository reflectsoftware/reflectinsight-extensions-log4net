using System;
using System.Reflection;
using log4net;

// log4net stuff
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Log4net_Sample
{
    class Program
    {
        static void TestLog4net()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            while (true)
            {
                Console.WriteLine("Press any key or 'q' to quit...");
                ConsoleKeyInfo k = Console.ReadKey();
                if (k.KeyChar == 'q')
                {
                    break;
                }

                Exception ex = new Exception("This is my test exception!");

                log.Info("[Enter]EnterMessage");
                log.Info("My Info");
                log.Info("My Info", ex);

                log.Debug("My Debug");
                log.Debug("My Debug", ex);

                log.Warn("My Warn");
                log.Warn("My Warn", ex);

                log.Error("My Error");
                log.Error("My Error", ex);

                log.Fatal("My Fatal");
                log.Fatal("My Fatal", ex);
                log.Info("[Exit]ExitMessage");
            }
        }

        static void Main(string[] args)
        {
            TestLog4net();
        }
    }
}
