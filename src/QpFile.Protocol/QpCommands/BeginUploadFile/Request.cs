using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace QpFile.Protocol.QpCommands.BeginUploadFile
{
    [DisplayName("Begin upload folder")]
    public class Request : AbstractQpSerializer<Request>, IQpCommandRequest<Request, Response>
    {
        protected override JsonTypeInfo<Request> GetTypeInfo() => BeginUploadFileCommandSerializerContext.Default.Request;
        public string Folder { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }
}
