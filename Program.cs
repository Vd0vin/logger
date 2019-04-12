using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

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
            PrepareAfterDecompress(outputPath, newOutputPath);
        }

        public static void PrepareForCompress(string inputPath, string outputPath)
        {
            var counter = 0;
            var flag = false;
            var line1 = "";
            var line2 = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();
            using (BinaryReader reader = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {
                while (reader.PeekChar() != -1)
                {
                    if (reader.PeekChar() == '\n') flag = true;
                    sb.Append(reader.ReadChar());
                    if (flag)
                    {
                        counter++;
                        if (counter % 2 != 0)
                        {
                            line1 = sb.ToString();
                            resultsb.Append(sb);
                        }
                        else
                        {
                            line2 = sb.ToString();

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

                            writer.Write(resultsb.ToString());
                            resultsb.Clear();
                        }
                        sb.Clear();
                        flag = false;
                    }
                }
            }
        }

        public static string CompareAndShrink(List <char> list1, List<char> list2)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder midsb = new StringBuilder();
            var counter = 0;
            var minList = list1.Count >= list2.Count ? list2 : list1;
            for (int i = 0; i < minList.Count; i++)
            {
                midsb.Append(list2[i]);
                if (list1[i] == list2[i]) counter++;
                if (list1[i] != list2[i] && counter > 4 || (counter > 4 && minList.Count - 1 == i))
                {
                    {
                        sb.Append('^');
                        sb.Append(Convert.ToString(counter, 2));
                        sb.Append('^');
                        sb.Append(list2[i]);
                        counter = 0;
                        midsb.Clear();
                    }
                }
            }
            midsb.Append('\r');
            sb.Append(midsb);
            midsb.Clear();
            if (minList.Count < list2.Count) sb.Append(list2.GetRange(list1.Count, list2.Count - list1.Count).ToArray());
            return sb.ToString()
;
        }

        public static void PrepareAfterDecompress(string outputPath, string newOutputPath)
        {
            var counter = 0;
            var flag = false;
            var line1 = "";
            var line2 = "";
            StringBuilder sb = new StringBuilder();
            StringBuilder resultsb = new StringBuilder();
            using (BinaryReader reader = new BinaryReader(File.Open(outputPath, FileMode.Open)))
            using (BinaryWriter writer = new BinaryWriter(File.Open(newOutputPath, FileMode.Create)))
            {
                while (reader.PeekChar() != -1)
                {
                    if (reader.PeekChar() == '\n') flag = true;
                    sb.Append(reader.ReadChar());
                    if (flag)
                    {
                        counter++;
                        if (counter % 2 != 0)
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
                            var list6 = array[3];
                            var list7 = line1.Substring(37).ToList();
                            var list8 = array[4];

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

                            writer.Write(resultsb.ToString());
                            resultsb.Clear();
                        }
                        sb.Clear();
                        flag = false;
                    }
                }
            }
        }
        public static List<char>[] ParseLine(string line)
        {
            var array = new List<char>[4];
            var startPosition = 0;
            var arrIndex = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\r')
                {
                    array[arrIndex] = line.Substring(startPosition, i - startPosition).ToList();
                    startPosition = i + 1;
                    arrIndex++;
                }
            }
            return array;
       }
        
    }
    }


