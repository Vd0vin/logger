using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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
            int counter = 0;
            StringBuilder temp = new StringBuilder();
            string line = string.Empty;
            StringBuilder preLine = new StringBuilder();
            using (StreamReader reader = new StreamReader(inputPath))
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                while (reader.Peek() >= 0)
                {
                    char c = (char)reader.Read();
                    if (c != '\n')
                    {
                        preLine.Append((char)reader.Read());
                    }
                    else
                    {
                        //create new line
                        line = preLine.ToString();
                        preLine.Clear();
                        var firstLine = "";
                        var dict = new Dictionary<int, string>();
                        //StringBuilder temp = new StringBuilder();
                        counter++;
                        if (counter % 2 != 0)
                        {
                            firstLine = line;
                            temp.Append(line);
                            continue;
                        }
                        StringBuilder preTemp = new StringBuilder();
                        var count = 0;
                        var minString = (firstLine.Length <= line.Length) ? firstLine : line;
                        var maxString = (firstLine.Length > line.Length) ? firstLine : line;

                        for (int i = 0; i < minString.Length; i += 1)
                        {

                            if (firstLine[i] == line[i])
                            {
                                count++;
                                preTemp.Append(line[i]);
                            }
                            else
                            {
                                if (count > 5)
                                {
                                    temp.Append("(" + (i - count) + "*" + count + ")");//index, asterisk, number of chars
                                }
                                else { temp.Append(preTemp); }
                                preTemp.Clear();
                                temp.Append(line[i]);
                                count = 0;
                            }
                            if (i == minString.Length - 1) temp.Append(preTemp);
                        }

                        //Console.WriteLine(s);
                        if (maxString == line) temp.Append(maxString.Substring(minString.Length));

                    }
                    
                }writer.Write(temp.ToString());
                
            }

            }

        public static void PrepareAfterDecompress(string outputPath, string newOutputPath)
        {
            var readText = File.ReadLines(outputPath);

            int counter = 0;
            var firstLine = "";
            StringBuilder temp = new StringBuilder();
            foreach (var line in readText)
            {
                counter++;
                if (counter % 2 != 0)
                {
                    firstLine = line;
                    temp.Append(line);
                    continue;
                }

                StringBuilder preTemp = new StringBuilder();
                var position = 0;
                var minString = (firstLine.Length <= line.Length) ? firstLine : line;
                var maxString = (firstLine.Length > line.Length) ? firstLine : line;
                var queueOfIndices = new Queue<int[]>();

                Regex regex = new Regex(@"\((\d*?)\*{1}(\d*?)\)");
                var matches = regex.Matches(line);

                foreach (Match match in matches)
                {
                    var matchIndex = match.Index;
                    var index = Int32.Parse(match.Groups[1].Value);
                    var length = Int32.Parse(match.Groups[2].Value);
                    if (position < matchIndex)
                    {
                        preTemp.Append(line.Substring(position, matchIndex - position));
                        position = matchIndex;
                    }

                    preTemp.Append(firstLine.Substring(index, length));
                }
                preTemp.Append(line.Substring(position));
                temp.Append(regex.Replace(preTemp.ToString(), ""));
            }
            using (StreamWriter file = new StreamWriter(newOutputPath))
                file.Write(temp.ToString());
        }
    }
}
