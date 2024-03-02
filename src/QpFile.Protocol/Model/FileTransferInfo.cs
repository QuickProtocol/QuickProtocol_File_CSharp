using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Protocol.Model
{
    public class FileTransferInfo : IDisposable
    {
        private Stream stream;

        public string Id { get; private set; }
        public string Path { get; private set; }
        public long Length { get; private set; }
        public long LastOffset { get; private set; }
        public DateTime LastTransferTime { get; private set; }
        public bool IsCompleted { get; private set; } = false;
        public string ErrorMessage { get; private set; }

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

        public void SetErrorMessage(string message)
        {
            ErrorMessage = message;
        }

        public void Dispose()
        {
            if (IsCompleted)
                return;
            stream.Dispose();
            IsCompleted = true;
        }
    }
}
