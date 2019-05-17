using System.IO;
namespace Kontur.LogPacker
{
    internal static class EntryPoint
    {
        public static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                var (inputFile, outputFile) = (args[0], args[1]);

                if (File.Exists(inputFile))
                {
                    Compress(inputFile, outputFile);
                    return;
                }
            }

            if (args.Length == 3 && args[0] == "-d")
            {
                var (inputFile, outputFile) = (args[1], args[2]);

                if (File.Exists(inputFile))
                {
                    Decompress(inputFile, outputFile);
                    return;
                }
            }
        }

        private static void Compress(string inputFile, string outputFile)
        {
            var tempOutputFile = "temp";
            Preparator.Preparator.PrepareForCompress(inputFile, tempOutputFile);
            using (var inputStream = File.OpenRead(tempOutputFile))
            using (var outputStream = File.OpenWrite(outputFile))
            {
                new GZipCompressor().Compress(inputStream, outputStream);
            }
        }

        private static void Decompress(string inputFile, string outputFile)
        {
            var tempOutputFile = "temp";
            using (var inputStream = File.OpenRead(inputFile))
            using (var outputStream = File.OpenWrite(tempOutputFile))
            {
                new GZipCompressor().Decompress(inputStream, outputStream);
            }
            Preparator.Preparator.PrepareAfterDecompress(tempOutputFile, outputFile);
        }  
    }
}