using System.IO;
using System.IO.Compression;

namespace Kontur.LogPacker
{
    internal class GZipCompressor
    {
        public void Compress(Stream inputStream, Stream outputStream)
        {
            using (var gzipStream = new BrotliStream(outputStream, CompressionLevel.Fastest, true)) 
                inputStream.CopyTo(gzipStream);
        }

        public void Decompress(Stream inputStream, Stream outputStream)
        {
            using (var gzipStream = new BrotliStream(inputStream, CompressionMode.Decompress, true))
                gzipStream.CopyTo(outputStream);
        }
    }
}