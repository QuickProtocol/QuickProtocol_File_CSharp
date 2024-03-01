using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace QpFile.Protocol.QpCommands.MovePath
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => MovePathCommandSerializerContext.Default.Response;
    }
}
