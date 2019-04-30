using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace Preparator
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = @"C:\C#work\Kontur\Kontur\Kontur.LogPacker.SelfCheck\example.log";
            string outputPath = @"C:\C#work\Kontur\Kontur\Kontur.LogPacker.SelfCheck\compressed.log";
            string newOutputPath = @"C:\C#work\Kontur\Kontur\Kontur.LogPacker.SelfCheck\decompressed.log";
            PrepareForCompress(inputPath, outputPath);
            //PrepareAfterDecompress(outputPath, newOutputPath);
        }

        public static void PrepareForCompress(string inputPath, string outputPath)
        {
            var counter = 0;
            var flag = false;
            var line1 = "";
            var line2 = "";
            var endOfLine = false;
            StringBuilder fullLineSb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();
            using (StreamReader reader = new StreamReader(File.Open(inputPath, FileMode.Open)))
            using (StreamWriter writer = new StreamWriter(File.Open(outputPath, FileMode.Create)))
            {
                while (reader.Peek() > -1)
                {
                    if ((char)reader.Peek() == '\n') flag = true;
                    sb.Append((char)reader.Read());
                    if (flag || reader.Peek() < 1)
                    {
                        if (counter % 12 == 0)
                        {
                            if (CheckDateFormat(sb.ToString(0, 24)))
                            {
                                if (fullLineSb.Length > 0)
                                {
                                    line1 = fullLineSb.ToString(); counter++;
                                    resultsb.Append(fullLineSb);
                                    fullLineSb.Clear();
                                }
                            }
                            fullLineSb.Append(sb);
                        }
                        else
                        {
                            if (CheckDateFormat(sb.ToString(0, 24)))
                            {
                                if (fullLineSb.Length > 0)
                                {
                                    line2 = fullLineSb.ToString(); counter++;
                                    fullLineSb.Clear();
                                }     
                            }
                            fullLineSb.Append(sb);
                            var list1 = line1.Substring(0, 24).ToList();
                            var list2 = line2.Substring(0, 24).ToList();
                            var list3 = line1.Substring(24, 7).ToList();
                            var list4 = line2.Substring(24, 7).ToList();
                            var list5 = line1.Substring(31, 6).ToList();
                            var list6 = line2.Substring(31, 6).ToList();
                            var list7 = line1.Substring(37).ToList();
                            var list8 = line2.Substring(37).ToList();

                            //var mid1 = CompareAndShrink(list1, list2);
                            //resultsb.Append(mid1);
                            resultsb.Append(CompareAndShrink(list1, list2));
                            //var mid2 = CompareAndShrink(list3, list4);
                            //resultsb.Append(mid2);
                            resultsb.Append(CompareAndShrink(list3, list4));
                            //var mid3 = CompareAndShrink(list5, list6);
                            //resultsb.Append(mid3);
                            resultsb.Append(CompareAndShrink(list5, list6));
                            //var mid4 = CompareAndShrink(list7, list8);
                            //resultsb.Append(mid4);
                            resultsb.Append(CompareAndShrink(list7, list8));

                        }
                        writer.Write(resultsb.ToString());
                        resultsb.Clear();
                        sb.Clear();
                        flag = false;
                    }
                }
                if (fullLineSb.Length > 0) { writer.Write(fullLineSb.ToString()); fullLineSb.Clear(); }
            }
        }

        public static string CompareAndShrink(List<char> list1, List<char> list2)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder midsb = new StringBuilder();
            var counter = 0;
            var minList = list1.Count >= list2.Count ? list2 : list1;
            for (int i = 0; i < minList.Count; i++)
            {
                midsb.Append(list2[i]);
                if (list1[i] == list2[i]) counter++;
                if (list1[i] != list2[i] && counter < 5) counter = 0;
                if (counter > 4 && (list1[i] != list2[i] || minList.Count - 1 == i))
                {
                    {
                        sb.Append('^');
                        sb.Append(Convert.ToString(counter, 2));
                        sb.Append('^');
                        if (list1[i] != list2[i]) sb.Append(list2[i]);
                        counter = 0;
                        midsb.Clear();
                    }
                }
            }
            sb.Append(midsb);
            midsb.Clear();
            if (minList.Count < list2.Count) sb.Append(list2.GetRange(list1.Count, list2.Count - list1.Count).ToArray());
            sb.Append('$');
            return sb.ToString();
        }

        public static void PrepareAfterDecompress(string outputPath, string newOutputPath)
        {
            var counter = 0;
            var flag = false;
            var line1 = "";
            var line2 = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();
            using (StreamReader reader = new StreamReader(File.Open(outputPath, FileMode.Open)))
            using (StreamWriter writer = new StreamWriter(File.Open(newOutputPath, FileMode.Create)))
            {
                while (reader.Peek() != -1)
                {
                    if ((char)reader.Peek() == '\n') flag = true;
                    sb.Append((char)reader.Read());
                    if (flag || reader.Peek() == -1)
                    {
                        if (counter % 12 == 0)
                        {
                            line1 = sb.ToString();
                            resultsb.Append(sb);
                        }
                        else
                        {
                            line2 = sb.ToString();
                            var array = ParseLine(line2);
                            var list1 = line1.Substring(0, 24).ToList();
                            var list2 = array[0];
                            var list3 = line1.Substring(24, 7).ToList();
                            var list4 = array[1];
                            var list5 = line1.Substring(31, 6).ToList();
                            var list6 = array[2];
                            var list7 = line1.Substring(37).ToList();
                            var list8 = array[3];

                            //var mid1 = CompareAndShrink(list1, list2);
                            //resultsb.Append(mid1);
                            resultsb.Append(CompareAndRestore(list1, list2));
                            //var mid2 = CompareAndShrink(list3, list4);
                            //resultsb.Append(mid2);
                            resultsb.Append(CompareAndRestore(list3, list4));
                            //var mid3 = CompareAndShrink(list5, list6);
                            //resultsb.Append(mid3);
                            resultsb.Append(CompareAndRestore(list5, list6));
                            //var mid4 = CompareAndShrink(list7, list8);
                            //resultsb.Append(mid4);
                            resultsb.Append(CompareAndRestore(list7, list8));
                        }
                        writer.Write(resultsb.ToString());
                        counter++;
                        resultsb.Clear();
                        sb.Clear();
                        flag = false;
                    }
                }
            }
        }

        public static List<char>[] ParseLine(string line)
        {
            var array = new List<char>[4];
            char[] charSeparators = new char[] { '$' };
            var arr = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr.Count(); i++)
            {
                var stringForList = arr[i].TrimEnd('$');
                array[i] = stringForList.ToList();
            }

            //var startPosition = 0;
            //var arrIndex = 0;
            //var length = 0;
            //for (int i = 0; i + 1 < line.Length; i++)
            //{
            //    length = i - startPosition;
            //    if (line[i + 1] == '$')
            //    {
            //        array[arrIndex] = line.Substring(startPosition, length).ToList();
            //        if(line.Length >= startPosition + length) startPosition += length;
            //        if (arrIndex < 3) arrIndex++;
            //    }
            //    if (arrIndex == 3) array[arrIndex] = line.Substring(startPosition).ToList();
            //}
            return array;
        }

        public static string CompareAndRestore(List<char> list1, List<char> list2)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder midsb = new StringBuilder();
            StringBuilder numbersb = new StringBuilder();
            var startIndex = 0;
            var flag = false;
            for (int i = 0; i < list2.Count; i++)
            {
                midsb.Append(list2[i]);

                if (list2[i] == '^')
                {
                    if (flag)
                    {
                        flag = false;
                        var length = Convert.ToInt32(numbersb.ToString(), 2);
                        if (startIndex + length <= list1.Count)
                            sb.Append(new string(list1.GetRange(startIndex, length).ToArray()));
                        midsb.Clear();
                        numbersb.Clear();
                        continue;
                    }
                    else { flag = true; startIndex = i; }
                }
                else
                {
                    if (flag) numbersb.Append(list2[i]);
                    else sb.Append(list2[i]);
                }
            }
            //if (!EndOfLine) sb.Append('$');// $
            return sb.ToString();
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

    }
}
