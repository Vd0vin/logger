using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace StructuralComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var UncompressedFile = @"C:\Users\Admin\Documents\Kontur\Kontur.LogPacker.SelfCheck\example.log";
            var DecompressedFile = @"C:\Users\Admin\Documents\Kontur\Kontur.LogPacker.SelfCheck\decompressed.log";
            var uncomprArr = File.ReadAllBytes(UncompressedFile);
            var decomprArr = File.ReadAllBytes(DecompressedFile);
            var equals = StructuralComparisons.StructuralEqualityComparer.Equals(
                uncomprArr, decomprArr);
            if (equals) Console.WriteLine("Good to go!");
            else
            {
                for (int i = 0; i < uncomprArr.Length; i++)
                {
                    if (uncomprArr[i] != decomprArr[i])
                    Console.WriteLine("should be: {0}, in fact: {1}", (char)uncomprArr[i], (char)decomprArr[i]);
                }

            }
                //throw new Exception("File was corrupted after decompression!");
        }
    }
}
