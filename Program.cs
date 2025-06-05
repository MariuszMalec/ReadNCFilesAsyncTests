using Serilog;
using Serilog.Events;
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
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                //.WriteTo.File("c:/tempnc/log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();


            //test szybkosci Async / Sync
            int i = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var testAsync = new TestAsync(logger);
            await testAsync.ViewErrors();
            logger.Information("finish async test");
            stopwatch.Stop();
            logger.Information($"time = {stopwatch.ElapsedMilliseconds}");

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            var testSync = new TestSync(logger);
            testSync.ViewErrors();
            logger.Information("finish sync test");
            stopwatch1.Stop();
            logger.Information($"time = {stopwatch1.ElapsedMilliseconds}");
        }
    }
}
