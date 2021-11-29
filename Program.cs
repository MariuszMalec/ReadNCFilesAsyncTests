using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReadNCFilesAsyncTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //test szybkosci Async / Sync
            int i = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await TestAsync.ViewErrors();
            Console.WriteLine("");
            stopwatch.Stop();
            Console.WriteLine($"time = {stopwatch.ElapsedMilliseconds}");
        }
    }
}
