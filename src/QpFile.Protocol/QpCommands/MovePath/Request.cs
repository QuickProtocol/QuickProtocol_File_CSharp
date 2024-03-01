using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace QpFile.Protocol.QpCommands.MovePath
{
    [DisplayName("Move path")]
    public class Request : AbstractQpSerializer<Request>, IQpCommandRequest<Request, Response>
    {
        protected override JsonTypeInfo<Request> GetTypeInfo() => MovePathCommandSerializerContext.Default.Request;
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
    }
}
