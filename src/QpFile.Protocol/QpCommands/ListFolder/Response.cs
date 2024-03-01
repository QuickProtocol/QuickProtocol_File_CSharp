using QpFile.Protocol.Model;
using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpCommands.ListFolder
{
    public class Response : AbstractQpSerializer<Response>
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        protected override JsonTypeInfo<Response> GetTypeInfo() => ListFolderCommandSerializerContext.Default.Response;

        public QpFolderInfo[] Folders { get; set; }
        public QpFileInfo[] Files { get; set; }
    }
}
