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
            string inputPath = @"D:\Kontur\Kontur\Kontur.LogPacker.SelfCheck\example.log";
            string outputPath = @"D:\Kontur\Kontur\Kontur.LogPacker.SelfCheck\compressed.log";
            string newOutputPath = @"D:\Kontur\Kontur\Kontur.LogPacker.SelfCheck\decompressed.log";
            PrepareForCompress(inputPath, outputPath);
            PrepareAfterDecompress(outputPath, newOutputPath);
        }

        public static void PrepareForCompress(string inputPath, string outputPath)
        {
            var counter = 0;
            var flag = false;
            var line1 = "";
            var line2 = "";
            StringBuilder fullLineSb = new StringBuilder();
            StringBuilder sb = new StringBuilder('$');
            StringBuilder resultsb = new StringBuilder();
            using (StreamReader reader = new StreamReader(File.Open(inputPath, FileMode.Open)))
            {
                char[] charArray = null;
                using (StreamWriter writer = new StreamWriter(File.Open(outputPath, FileMode.Create)))
                {
                    while (reader.Peek() > -1)
                    {
                        charArray = new char[1024];
                        reader.Read(charArray, 0, charArray.Length);
                        for (int i = 0; i < charArray.Length; i++)
                        {
                            sb.Append(charArray[i]);
                            if (charArray[i] == '\n') flag = true;

                            if (flag)
                            {
                                if (counter % 12 == 0)
                                {
                                    if (CheckDateFormat(sb.ToString(0, 24)))
                                    {
                                        if (fullLineSb.Length > 0)
                                        {
                                            fullLineSb.Append('$');
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
                                    if (line1.Length > 0 && line2.Length > 0)
                                    {
                                        var list1 = line1.Substring(0, 24);
                                        var list2 = line2.Substring(0, 24);
                                        var list3 = ParseNumber(line1);
                                        var list4 = ParseNumber(line2);
                                        var list5 = line1.Substring(24 + list3.Length);
                                        var list6 = line2.Substring(24 + list4.Length);

                                        //var mid1 = CompareAndShrink(list1, list2);
                                        //resultsb.Append(mid1);
                                        resultsb.Append(CompareAndShrink(list1, list2));
                                        //var mid2 = CompareAndShrink(list3, list4);
                                        //resultsb.Append(mid2);
                                        resultsb.Append(ShrinkNumber(list3, list4));
                                        //var mid3 = CompareAndShrink(list5, list6);
                                        //resultsb.Append(mid3);
                                        resultsb.Append(CompareAndShrink(list5, list6));
                                        //var mid4 = CompareAndShrink(list7, list8);
                                        //resultsb.Append(mid4);
                                    }
                                }
                                writer.Write(resultsb.ToString());
                                line2 = "";
                                resultsb.Clear();
                                sb.Clear();
                                flag = false;
                            }
                        }
                    }
                    if (fullLineSb.Length > 0) { writer.Write(fullLineSb.ToString()); fullLineSb.Clear(); }
                }
            }
        }

        public static string CompareAndShrink(string list1, string list2)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder midsb = new StringBuilder();
            var counter = 0;
            var minList = list1.Length >= list2.Length ? list2 : list1;
            for (int i = 0; i < minList.Length; i++)
            {
                midsb.Append(list2[i]);
                if (list1[i] == list2[i] && list2[i] != '\n') counter++;
                if (list2[i] == '\n') counter = 0;
                if (list1[i] != list2[i] && counter < 5)
                {
                    counter = 0;
                    sb.Append(midsb);
                    midsb.Clear();
                }
                if (counter > 4 && (list1[i] != list2[i] || minList.Length - 1 == i))
                {
                    {
                        sb.Append('^');
                        //sb.Append(Convert.ToString(counter, 2));
                        sb.Append(counter);
                        sb.Append('^');
                        if (list1[i] != list2[i]) sb.Append(list2[i]);
                        counter = 0;
                        midsb.Clear();
                    }
                }
            }
            sb.Append(midsb);
            midsb.Clear();
            if (minList.Length < list2.Length) sb.Append(list2.Substring(list1.Length, list2.Length - list1.Length));
            sb.Append('$');
            return sb.ToString();
        }

        public static string ShrinkNumber(string list1, string list2) // refactor later
        {
            var result = new StringBuilder();
            StringBuilder number1 = new StringBuilder();
            StringBuilder number2 = new StringBuilder();
            for (int i = 0; i < list1.Length; i++)
            {
                if (char.IsDigit(list1[i])) number1.Append(list1[i]);
                else break;
            }

            for (int i = 0; i < list2.Length; i++)
            {
                if (char.IsDigit(list2[i])) number2.Append(list2[i]);
                else break;
            }
            var difference = int.Parse(number2.ToString()) - int.Parse(number1.ToString());
            result.Append(difference);
            if (number2.Length < 6) result.Append(' ', 6 - number2.Length);
            result.Append(" $");
            return result.ToString();
        }

        public static void PrepareAfterDecompress(string outputPath, string newOutputPath)
        {
            var counter = 0;
            var preflag = false;
            var flag = false;
            var line1 = "";
            var line2 = "";
            StringBuilder fullLineSb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();
            using (StreamReader reader = new StreamReader(File.Open(outputPath, FileMode.Open)))
            {
                char[] charArray = null;
                using (StreamWriter writer = new StreamWriter(File.Open(newOutputPath, FileMode.Create)))
                {
                    while (reader.Peek() != -1)
                    {
                        charArray = new char[1024];
                        reader.Read(charArray, 0, charArray.Length);
                        for (int i = 0; i < charArray.Length; i++)
                        {
                            if (charArray[i] == '\0') break; 
                            if (charArray[i] == '\n') preflag = true;
                            if (preflag && charArray[i] == '$')
                            {
                                flag = true;
                            }
                            else
                            {
                                sb.Append(charArray[i]);
                                flag = false;
                            }
                            if (flag)
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
                                    if (array.Length < 3)
                                    {
                                        foreach (var item in array)
                                            resultsb.Append(item);
                                        //foreach (var subitem in item)
                                        //resultsb.Append(subitem);
                                    }
                                    else
                                    {
                                        var list1 = line1.Substring(0, 24);
                                        var list2 = array[0];
                                        var list3 = ParseNumber(line1);
                                        var list4 = array[1];
                                        var list5 = line1.Substring(24 + list3.Length);
                                        var list6 = array[2];

                                        //var mid1 = CompareAndRestore(list1, list2);
                                        //resultsb.Append(mid1);
                                        resultsb.Append(CompareAndRestore(list1, list2));
                                        //var mid2 = CompareAndRestore(list3, list4);
                                        //resultsb.Append(mid2);
                                        resultsb.Append(RestoreNumber(list3, list4));
                                        //var mid3 = CompareAndRestore(list5, list6);
                                        //resultsb.Append(mid3);
                                        resultsb.Append(CompareAndRestore(list5, list6));
                                        //var mid4 = CompareAndRestore(list7, list8);
                                        //resultsb.Append(mid4);
                                    }
                                }
                                if (resultsb.Length >= 200)
                                {
                                    writer.Write(resultsb.ToString());
                                    resultsb.Clear();
                                }
                                counter++;
                                sb.Clear();
                                flag = false;
                                preflag = false;
                            }
                        }
                    }
                    if (resultsb.Length > 0 || sb.Length > 0)
                    {
                        resultsb.Append(sb);
                        writer.Write(resultsb.ToString());
                    }
                }
            }
        }

        public static string[] ParseLine(string line)
        {
            var arr = line.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
            var array = new string[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                var stringForList = arr[i].TrimEnd('$');
                array[i] = stringForList;
            }
            return array;
        }

        public static string CompareAndRestore(string list1, string list2)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder midsb = new StringBuilder();
            StringBuilder numbersb = new StringBuilder();
            var startIndex = 0;
            var flag = false;
            for (int i = 0; i < list2.Length; i++)
            {
                midsb.Append(list2[i]);
                if (sb.Length > 0) startIndex = sb.Length;

                if (list2[i] == '^')
                {
                    if (flag)
                    {
                        flag = false;
                        var length = Convert.ToInt32(numbersb.ToString());
                        if (startIndex + length <= list1.Length)
                        {
                            sb.Append(list1.Substring(startIndex, length));
                        }
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

        public static string ParseNumber(string line) // helps determine length of number part
        {
            StringBuilder result = new StringBuilder();
            var lengthLimit = line.Length > 44 ? 20 : line.Length - 24;
            var numberChunk = line.Substring(24, lengthLimit);
            for (int i = 0; i < lengthLimit; i++)
            {
                if (char.IsDigit(numberChunk[i])) result.Append(numberChunk[i]);
                else
                {
                    if (result.Length < 6) result.Append(' ', 6 - result.Length);
                    break;
                }
            }
            result.Append(' ');
            return result.ToString();
        }

        public static string RestoreNumber(string list1, string list2)
        {
            var result = new StringBuilder();
            StringBuilder number1 = new StringBuilder();
            StringBuilder number2 = new StringBuilder();
            for (int i = 0; i < list1.Length; i++)
            {
                if (char.IsDigit(list1[i])) number1.Append(list1[i]);
                else break;
            }

            for (int i = 0; i < list2.Length; i++)
            {
                if (char.IsDigit(list2[i])) number2.Append(list2[i]);
                else break;
            }
            var sum = int.Parse(number2.ToString()) + int.Parse(number1.ToString());
            result.Append(sum);
            if (result.Length < 6) result.Append(' ', 6 - result.Length);
            result.Append(' ');
            return result.ToString();
        }

    }
}