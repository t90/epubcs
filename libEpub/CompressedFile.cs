using System;
using System.IO;

namespace libEpub
{
    public class CompressedFile : IDisposable
    {
        public Stream ContentsStream { get; private set; }

        public CompressedFile(Stream contentsStream)
        {
            ContentsStream = contentsStream;
        }

        public string Path;
        public string ContentType;

        public void Save(Stream stream)
        {
            var buffer = new byte[512];
            int read;

            while ((read = ContentsStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, read);
            }
        }

        public void Save(string path)
        {
            using (var f = File.Create(path))
            {
                Save(f);
            }
        }

        public void Dispose()
        {
            ContentsStream.Dispose();
        }
    }
}