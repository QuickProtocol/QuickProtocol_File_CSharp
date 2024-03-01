using Quick.Protocol;
using System.Text.Json.Serialization.Metadata;

namespace QpFile.Protocol.QpNotices
{
    public class FilePartInfo : AbstractQpSerializer<FilePartInfo>
    {
        protected override JsonTypeInfo<FilePartInfo> GetTypeInfo() => NoticesSerializerContext.Default.FilePartInfo;
        public string FileId { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
        public byte[] FileData { get; set; }
    }
}
