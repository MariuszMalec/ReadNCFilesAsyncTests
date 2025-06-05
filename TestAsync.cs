using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReadNCFilesAsyncTests
{
    public class TestAsync
    {
        private static string SourceFiles = (@"SourceFiles");
        private ILogger _logger;

        public TestAsync(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ViewErrors()
        {
            List<Task> tasks = new List<Task>();
            var files = GetFiles(SourceFiles);
            foreach (var file in files)
            {
                tasks.Add(checkM17(file));
                tasks.Add(checkM6(file));
                tasks.Add(check_E_ZDARZ(file, "E_ZDARZ=3"));                
                tasks.Add(checkSyntaxError(file));
                //tasks.Add(Task.Run(() => checklimitedPositionYZ(file)));
            }
            await Task.WhenAll(tasks);
        }
        private string ValidateNcLine(string line)
        {
            var result = string.Empty;
            if (line.Contains("MSG") ||
                line.Contains("DELTA") ||
                line.Contains("RAPORT") ||
                line.Contains("TRAFOOF") ||
                line.Contains("STOPRE") ||
                line.Contains("T=") ||
                line.Contains("FGROUP") ||
                line.Contains("TRANS") ||
                line.Contains("FFWON") ||
                line.Contains("E_ZDARZ") ||
                line.Contains("FNORM") ||
                line.Contains("CYCLE832"))
                return result;
            if (line.StartsWith(";"))
                return result;
            if (line.StartsWith("N"))
            {
                var splitN = line.Split(' ');
                if (splitN.Length > 1)
                    if (splitN[1].StartsWith(";"))
                        return result;
            }
            if (!line.StartsWith(";") && line.Contains(";"))
            {
                return line.Split(";")[0];
            }
            return line;
        }
        private async Task<string> checkSyntaxError(string file)
        {
            if (File.Exists(file))
            {
                _logger.Debug($"{file}  ... Check checkSyntaxError ");
                var errors = string.Empty;
                var words = new List<string>() { "X", "Y", "Z", "A", "B", "F" };

                using (StreamReader sr = File.OpenText(file))
                {
                    string s = String.Empty;

                    while ((s = await sr.ReadLineAsync()) != null)
                    {
                        var line = ValidateNcLine(s);

                        if (line != "")
                        {
                            var xValue = line.Split(' ');
                            if (xValue.Length > 0)
                            {
                                double number;
                                foreach (var x in xValue)
                                {
                                    foreach (string word in words)
                                    {
                                        if (x.StartsWith(word))
                                        {
                                            if (!Double.TryParse(x.Substring(1), out number))
                                            {
                                                if (number == 0)
                                                {
                                                    _logger.Error($"{Path.GetFileName(file)}, Os {word}, Nie mozna z parsowac lini: {line} !");
                                                    return ($"{Path.GetFileName(file)}, Nie mozna z parsowac lini: {line} !");//TODO pomimo bledow mam po wyjsciu pusta liste?
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return errors;
            }
            return string.Empty;
        }

        private async Task<string> check_E_ZDARZ(string fileName, string e_zdarz)
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
                        _logger.Debug($"{fileName}  ... Check E_ZDARZ={e_zdarz} ");
                        using (StreamReader sr = File.OpenText(fileName))
                        {
                            string s = String.Empty;
                            while ((s = await sr.ReadLineAsync()) != null)
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
                                _logger.Error(txtwarnig);
                            }
                        }
                    }
                }
            }
            else { txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono check_E_ZDARZ={e_zdarz}"); }

            return txtwarnig;
        }

        private async Task<string> checkM6(string fileName)
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
                _logger.Debug($"{fileName}  ... Check {searchtext}");
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = String.Empty;
                    while ((s = await sr.ReadLineAsync()) != null)
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
                        _logger.Error(txtwarnig);
                    }
                }
            }
            else { txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono braku wymiany narzedzia"); }

            return txtwarnig;
        }

        private async Task<string> checkM17(string fileName)
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
                _logger.Debug($"{fileName}  ... Check {searchtext} ");
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = String.Empty;
                    while ((s = await sr.ReadLineAsync()) != null)
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
                        _logger.Error(txtwarnig);
                    }
                }
            }
            else
            {
                txtwarnig = ($"BRAK.PRG => {fileName} nie sprawdzono M17");
            }
            return txtwarnig;
        }
        private string checklimitedPositionYZ(string file)
        {
            //MessageBox.Show("SPRAWDZANIE PRZEKROCZEN W OSI " + axis,"UWAGA!",MessageBoxButtons.OK, MessageBoxIcon.Information);
            bool warning = false;
            string txtwarnig = "";
            if (File.Exists(file) && file.Contains(".NC"))
            {
                _logger.Debug($"{file}  ... Check limited axis");
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
                    _logger.Error(txtwarnig);
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

        private List<string> GetFiles(string dir)
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
