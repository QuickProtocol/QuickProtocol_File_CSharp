using QpFile.Protocol.Model;
using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.CreateFolder
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => CreateFolderCommandSerializerContext.Default.Response;
    }
}
