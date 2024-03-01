using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.EndDownloadFile
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => EndDownloadFileCommandSerializerContext.Default.Response;
    }
}
