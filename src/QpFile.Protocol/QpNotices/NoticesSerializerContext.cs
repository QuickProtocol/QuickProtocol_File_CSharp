using System.Text.Json.Serialization;

namespace QpFile.Protocol.QpNotices;

[JsonSerializable(typeof(FilePartInfo))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class NoticesSerializerContext : JsonSerializerContext { }