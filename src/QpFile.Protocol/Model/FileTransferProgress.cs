using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Protocol.Model
{
    public struct FileTransferProgress
    {
        public long Offset { get; set; }
        public long Length { get; set; }
    }
}
