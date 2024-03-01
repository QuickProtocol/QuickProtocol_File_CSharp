using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Server
{
    public class FileTransferInfo : IDisposable
    {
        private Stream stream;

        public string Id { get; private set; }
        public string Path { get; private set; }
        public long Length { get; private set; }
        public long LastOffset { get; private set; }
        public DateTime LastTransferTime { get; private set; }
        public bool IsDisposed { get; private set; } = false;

        public FileTransferInfo(string id, string path, long length, Stream stream)
        {
            this.stream = stream;
            Id = id;
            Path = path;
            Length = length;
            LastOffset = 0;
            LastTransferTime = DateTime.Now;
        }

        public void Write(byte[] buffer)
        {
            stream.Write(buffer);
            LastOffset += buffer.Length;
            LastTransferTime = DateTime.Now;
        }

        public void Dispose()
        {
            stream.Dispose();
            IsDisposed = true;
        }
    }
}
