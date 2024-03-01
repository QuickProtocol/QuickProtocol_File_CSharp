using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.BeginUploadFile
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => BeginUploadFileCommandSerializerContext.Default.Response;
        public string FileId { get; set; }
    }
}
