using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace QpFile.Protocol.QpCommands.EndDownloadFile
{
    [DisplayName("End download folder")]
    public class Request : AbstractQpSerializer<Request>, IQpCommandRequest<Request, Response>
    {
        protected override JsonTypeInfo<Request> GetTypeInfo() => EndDownloadFileCommandSerializerContext.Default.Request;
        public string FileId { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
