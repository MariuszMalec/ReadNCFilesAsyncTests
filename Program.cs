using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace ReadNCFilesAsyncTests
{
    class Program
    {

        static async Task Main(string[] args)
        {


            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.WriteTo.File("c:/tempnc/log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();


            //test szybkosci Async / Sync
            int i = 0;
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //await TestAsync.ViewErrors();
            //Console.WriteLine("");
            //stopwatch.Stop();
            //Console.WriteLine($"time = {stopwatch.ElapsedMilliseconds}");

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            var testSync = new TestSync(logger);
            testSync.ViewErrors();
            logger.Information("");
            stopwatch1.Stop();
            logger.Information($"time = {stopwatch1.ElapsedMilliseconds}");
        }
    }
}
