using System.Text.Json.Serialization;

namespace QpFile.Protocol.QpCommands;

[JsonSerializable(typeof(ListFolder.Request))]
[JsonSerializable(typeof(ListFolder.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class ListFolderCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(Login.Request))]
[JsonSerializable(typeof(Login.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class LoginCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(CreateFolder.Request))]
[JsonSerializable(typeof(CreateFolder.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class CreateFolderCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(Delete.Request))]
[JsonSerializable(typeof(Delete.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class DeleteCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(BeginUploadFile.Request))]
[JsonSerializable(typeof(BeginUploadFile.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class BeginUploadFileCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(EndUploadFile.Request))]
[JsonSerializable(typeof(EndUploadFile.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class EndUploadFileCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(BeginDownloadFile.Request))]
[JsonSerializable(typeof(BeginDownloadFile.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class BeginDownloadFileCommandSerializerContext : JsonSerializerContext { }

[JsonSerializable(typeof(EndDownloadFile.Request))]
[JsonSerializable(typeof(EndDownloadFile.Response))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class EndDownloadFileCommandSerializerContext : JsonSerializerContext { }

