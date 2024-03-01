using QpFile.Protocol.Model;
using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.Delete
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => DeleteCommandSerializerContext.Default.Response;
    }
}
