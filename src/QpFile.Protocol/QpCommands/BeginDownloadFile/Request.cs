using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace QpFile.Protocol.QpCommands.BeginDownloadFile
{
    [DisplayName("Begin download file")]
    public class Request : AbstractQpSerializer<Request>, IQpCommandRequest<Request, Response>
    {
        protected override JsonTypeInfo<Request> GetTypeInfo() => BeginDownloadFileCommandSerializerContext.Default.Request;
        public string Path { get; set; }
    }
}
