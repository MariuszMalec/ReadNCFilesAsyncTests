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

            List<long> timelist = new List<long>(new long[] { });

            //test szybkosci Async / Sync
            int i = 0;
            timelist.Clear();
            do
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await TestAsync.ViewMsg();
                //TestBezAsync.ViewMsg();

                Console.WriteLine("");
                stopwatch.Stop();
                timelist.Add(stopwatch.ElapsedMilliseconds);
                i++;
            }
            while (i < 5);
            Console.WriteLine($"The average is {timelist.Average() / 100}");
        }
    }
}
