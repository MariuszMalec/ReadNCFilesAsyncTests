using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReadNCFilesAsyncTests
{
    public class TestAsync
    {
        private static string SourceFiles = (@"SourceFiles");
        public static async Task ViewErrors()
        {
            List<Task> tasks = new List<Task>();
            var files = GetFiles(SourceFiles);
            foreach (var file in files)
            {
                //tasks.Add(???????);

                checkM17(file);
                checkM6(file);
                check_E_ZDARZ(file, "E_ZDARZ=3");
                checklimitedPositionYZ(file);

            }
            await Task.WhenAll(tasks);
        }

        private static string check_E_ZDARZ(string fileName, string e_zdarz)
        {
            bool warning = false;
            string txtwarnig = "";
            if (File.Exists(fileName))
            {
                warning = false;
                if (fileName.Contains("35.SPF") || fileName.Contains("49.SPF"))
                {
                    if (File.Exists(fileName))
                    {
                        Console.WriteLine($"{fileName}  ... Check E_ZDARZ={e_zdarz} ");
                        using (StreamReader sr = File.OpenText(fileName))
                        {
                            string s = String.Empty;
                            while ((s = sr.ReadLine()) != null)
                            {
                                //do minimal amount of work here
                                if (s.Contains(e_zdarz))
                                {
                                    warning = false;
                                    break;
                                }
                                else
                                {
                                    warning = true;
                                }
                            }
                            if (warning == true)
                            {
                                txtwarnig = ($"W programie {fileName} brak E_ZDARZ={e_zdarz}");
                            }
                        }
                    }
                }
            }
            else { txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono check_E_ZDARZ={e_zdarz}"); }
            return txtwarnig;
        }

        private static string checkM6(string fileName)
        {
            bool warning = false;
            string txtwarnig = "";
            if (File.Exists(fileName) && fileName.Contains(".NC"))
            {
                warning = false;
                string searchtext = "";
                if (fileName.Contains(".NC"))
                {
                    searchtext = "M6";
                }
                else
                {
                    searchtext = "L9006";
                }
                Console.WriteLine($"{fileName}  ... Check {searchtext}");
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        //do minimal amount of work here
                        if (s.Contains(searchtext))
                        {
                            warning = false;
                            break;
                        }
                        else
                        {
                            warning = true;
                        }
                    }
                    if (warning == true)
                    {
                        txtwarnig = ($"W programie {fileName} brak wymiany narzedzia {searchtext}!!!");
                    }
                }
            }
            else { txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono braku wymiany narzedzia"); }
            return txtwarnig;
        }

        private static string checkM17(string fileName)
        {
            bool warning = false;
            string txtwarnig = "";
            if (File.Exists(fileName))
            {
                warning = false;
                string searchtext = "";
                if (fileName != ".NC")
                {
                    searchtext = "M17";
                }
                else
                {
                    searchtext = "M30";
                }
                Console.WriteLine($"{fileName}  ... Check {searchtext} ");
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        //do minimal amount of work here
                        if (s.Contains(searchtext))
                        {
                            warning = false;
                            break;
                        }
                        else
                        {
                            warning = true;
                        }
                    }
                    if (warning == true)
                    {
                        txtwarnig = ($"W programie {fileName} brak {searchtext}!!");
                    }
                }
            }
            else
            {
                txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono M17");
            }
            return txtwarnig;
        }
        private static string checklimitedPositionYZ(string file)
        {
            //MessageBox.Show("SPRAWDZANIE PRZEKROCZEN W OSI " + axis,"UWAGA!",MessageBoxButtons.OK, MessageBoxIcon.Information);
            bool warning = false;
            string txtwarnig = "";
            if (File.Exists(file) && file.Contains(".NC"))
            {
                Console.WriteLine($"{file}  ... Check limited axis");
                var lines = File.ReadAllLines(file);
                string checkline = "";
                string takeaxisstring = "";
                float takeaxisvalue = 0;
                float AxisStopZ = 0;
                float AxisStopY = 0;
                //sprawdzenie czy dobrze wpisano osie!!!
                AxisStopZ = Convert.ToSingle(590.0);
                AxisStopY = Convert.ToSingle(250.0);
                //ostrzezenia przed blednym wpisaniem wartosci
                if (AxisStopY < 0.0)
                {
                    txtwarnig = ("SPR.PRG => Nie moze byc wpisana wartosc ujemna osi Y!!");
                }
                string axisZ = "";
                string axisY = "";
                //string Zmax = "";
                //string Ymin = "";
                axisZ = "Z";
                //Zmax = textBox2.Text;
                axisY = "Y";
                //Ymin = textBox3.Text;
                List<string> listAxis = new List<string>(new string[] { });
                List<string> listaprzekroczenZ = new List<string>(new string[] { });
                List<string> listaprzekroczenY = new List<string>(new string[] { });
                warning = false;
                //MessageBox.Show(AxisStop.ToString(),"UWAGA!! ",MessageBoxButtons.OK, MessageBoxIcon.Information);
                foreach (var line in lines)
                {
                    if (line.Contains(axisZ))
                    {
                        checkline = line.Replace(" ", "");
                        if (checkline.Contains("FGROUP") || checkline.Contains("ZTGW") || checkline.Contains("NARZ")
                            || checkline.Contains("NAZWA") || checkline.Contains("NOZKA") || checkline.Contains("BANDAZ")
                            || checkline.Contains("ZGR") || checkline.Contains("ZACIAG") || checkline.Contains("LUZ") ||
                            checkline.Contains("ZIGNOROWANY") || checkline.Contains("ZW") || checkline.Contains("SZ")
                            || checkline.Contains("FAZ") || checkline.Contains("Z=") || checkline.StartsWith(";"))
                        {
                            warning = false;
                        }
                        else
                        {
                            try
                            {
                                string[] subs = line.Split(' ', ';');
                                foreach (string item in subs)
                                {
                                    if (item.Contains("Z"))
                                    {
                                        //MessageBox.Show(item.ToString());
                                        takeaxisstring = item.Replace("Z", "");
                                        takeaxisvalue = Convert.ToSingle(takeaxisstring);//kasuje .000 ??
                                                                                         //MessageBox.Show(takeaxisvalue.ToString());
                                        if (AxisStopZ > 0.0)
                                        {
                                            if ((takeaxisvalue > AxisStopZ) & (axisZ == "Z"))
                                            {
                                                listAxis.Add("SPR.PRG =>" + file + " => przekroczono " + axisZ + ", PATRZ BLOK:" + line);
                                                listaprzekroczenZ.Add(line);
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                            //nie dziala gdy brak po osi Z spacji!
                        }
                    }
                    if (line.Contains(axisY))
                    {
                        checkline = line.Replace(" ", "");
                        if (checkline.Contains("FGROUP") || checkline.Contains("ZTGW") || checkline.Contains("NARZ")
                            || checkline.Contains("NAZWA") || checkline.Contains("NOZKA") || checkline.Contains("BANDAZ")
                            || checkline.Contains("ZGR") || checkline.Contains("ZACIAG") || checkline.Contains("LUZ") ||
                            checkline.Contains("ZIGNOROWANY") || checkline.Contains("ZW") || checkline.Contains("SZ")
                            || checkline.Contains("FAZ") || checkline.Contains("Z=") || checkline.StartsWith(";"))
                        {
                            warning = false;
                        }
                        else
                        {
                            try
                            {
                                checkline = line + " ";//trzeba dodac na koncu spacje bo nie ma konca lini
                                string pattern = axisY;//wycigniecie danej osi do spacji , UWAGA gdy brak spacji nie dziala prawidlowo
                                int index = 0;
                                while (true)
                                {
                                    int a = checkline.IndexOf(pattern, index);
                                    if (a == -1)
                                        break;
                                    index = a + pattern.Length;
                                    int b = checkline.IndexOf(" ", index);
                                    takeaxisstring = checkline.Substring(index, b - index);
                                    takeaxisvalue = Convert.ToSingle(takeaxisstring);//zmien na liczbe
                                    //MessageBox.Show(takeaxisstring.ToString(),"UWAGA!! ",MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (AxisStopY > 0.0)
                                    {
                                        if ((takeaxisvalue < -AxisStopY) & (axisY == "Y") & AxisStopY > 0.0)
                                        {
                                            listAxis.Add("SPR.PRG =>" + file + " => przekroczono " + axisY + ", PATRZ BLOK:" + line);
                                            listaprzekroczenY.Add(line);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                int count = 0;
                foreach (string element in listAxis)
                {
                    txtwarnig = (listAxis[count]);
                    count++;
                }
                if (warning == true)
                {
                    //listwarnings.Add("W programie przekroczona os Zmax!!");
                    //MessageBox.Show("W programie przekroczona os Zmax","UWAGA!! ",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else { txtwarnig = ($"BRAK.PRG => {file} nie sprawdzono checklimitedPositionYZ"); }
            return txtwarnig;
        }

        private static List<string> GetFiles(string dir)
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
