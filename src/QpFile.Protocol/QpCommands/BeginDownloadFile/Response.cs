using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.BeginDownloadFile
{
    public class Response : AbstractQpSerializer<Response>
    {
        protected override JsonTypeInfo<Response> GetTypeInfo() => BeginDownloadFileCommandSerializerContext.Default.Response;
        public string FileId { get; set; }
        public long Length { get; set; }
    }
}
