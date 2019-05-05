using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace ErsatsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = @"C:\Users\Admin\Documents\Kontur\Kontur.LogPacker.SelfCheck\exampleR.log";
            string outputPath = @"C:\Users\Admin\Documents\Kontur\Kontur.LogPacker.SelfCheck\example.log";
            PopulateFile(inputPath, outputPath);
        }

        public static void PopulateFile(string inputPath, string outputPath)
        {
            var digito = 812;
            var counter = 0;
            
            var flag = false;
            var line1 = "";
            StringBuilder fullLineSb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();

            using (StreamReader reader = new StreamReader(File.Open(inputPath, FileMode.Open)))
            using (StreamWriter writer = new StreamWriter(File.Open(outputPath, FileMode.Create)))
            {
                for (int i = 0; i < 133; i++)
                {
                    reader.DiscardBufferedData();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.BaseStream.Position = 0;
                    while (reader.Peek() > -1)
                    {

                        if ((char)reader.Peek() == '\n') flag = true;
                        sb.Append((char)reader.Read());
                        if (flag || reader.Peek() < 1)
                        {
                            if (CheckDateFormat(sb.ToString(0, 24)))
                            {
                                if (fullLineSb.Length > 0)
                                {
                                    line1 = fullLineSb.ToString(); counter++;
                                    //resultsb.Append(fullLineSb);
                                    fullLineSb.Clear();
                                }
                            }
                            fullLineSb.Append(sb);
                            
                            if (line1.Length > 0)
                            {
                                var list1 = line1.Substring(0, 24);

                                var list3 = ParseNumber(digito);

                                var list5 = line1.Substring(24 + list3.Count());

                                //var mid1 = CompareAndShrink(list1, list2);
                                //resultsb.Append(mid1);
                                resultsb.Append(list1);
                                //var mid2 = CompareAndShrink(list3, list4);
                                //resultsb.Append(mid2);
                                resultsb.Append(list3);
                                //var mid3 = CompareAndShrink(list5, list6);
                                //resultsb.Append(mid3);
                                resultsb.Append(list5);
                                //var mid4 = CompareAndShrink(list7, list8);
                                //resultsb.Append(mid4);
                            }
                        
                        writer.Write(resultsb.ToString());
                            digito++;
                        line1 = "";
                        resultsb.Clear();
                        sb.Clear();
                        flag = false;
                        }
                    }

                    //if (fullLineSb.Length > 0) { writer.Write(fullLineSb.ToString()); fullLineSb.Clear(); }
                }
            }
        }

        public static int MakeNumberUp()
        {
            
            var random = new Random();
            var randman = random.Next(20);
            var result = randman + randman;
            return result;
        }

        public static bool CheckDateFormat(string s)
        {
            var dateSubstring = s.Length >= 24 ? s.Substring(0, 24) : "";
            DateTime dt;
            var format = "yyyy-MM-dd HH:mm:ss,fff ";
            return DateTime.TryParseExact(dateSubstring, format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dt);
        }

        public static string ParseNumber(int number) // helps determine length of number part
        {
            string myString = number.ToString();
            StringBuilder result = new StringBuilder(myString);
            if (result.Length < 6) result.Append(' ', 6 - result.Length);
            result.Append(' ');
            return result.ToString();
        }
    }
}


