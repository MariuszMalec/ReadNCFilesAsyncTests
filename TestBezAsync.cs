using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadNCFilesAsyncTests
{
    public class TestBezAsync
    {
        private static string SourceFiles = (@"SourceFiles");
        public static void ViewMsg()
        {
            var files = GetFiles(SourceFiles);
            List<string> tasks = new List<string>();
            foreach (var file in files)
            {
                CreateMsgSummary(file);
            }
        }
        private static void CreateMsgSummary(string file)
        {
            Console.WriteLine($"Tworzę podsumowanie MSG {file}.");
            LongRunningOperation(file);
            Console.WriteLine($"Podsumowanie Msg {file} utworzone.");
        }
        private static void LongRunningOperation(string file)
        {
            var nc = GetNcLinesFromNC(file);
            var valid = nc.Where(n => n.Contains("MSG") && n.Contains("TLID")).Select(n => n);
            valid.ToList().ForEach(n => Console.WriteLine($"{n}"));
        }

        public static List<string> GetNcLinesFromNC(string file)
        {
            var ncinfo = new List<string>();
            if (File.Exists(file))
            {
                string[] linesReaded = File.ReadAllLines(file);
                int lineNb = 1;

                ncinfo.Clear();
                foreach (string line in linesReaded)
                {
                    if (line != "")
                    {
                        string[] temp = { lineNb.ToString(), line };
                        ncinfo.Add(line);
                        lineNb++;
                    }
                }
            }
            return ncinfo;
        }
        public static List<string> GetFiles(string dir)
        {
            List<string> listsubprogramms = new List<string>(new string[] { });
            string[] ncFiles = Directory.GetFiles(dir);
            foreach (string ncFile in ncFiles)
            {
                string extension = Path.GetExtension(Path.GetFileName(ncFile));
                if (extension.Contains("SPF") || extension.Contains("spf") || extension.Contains("NC"))
                {
                    listsubprogramms.Add(ncFile);
                }
            }
            return listsubprogramms;
        }
    }
}
