using Quick.Protocol;

namespace QpFile.Protocol
{
    public class Instruction
    {
        public static QpInstruction Instance = new QpInstruction()
        {
            Id = typeof(Instruction).Namespace,
            Name = "QpFile Protocol",
            CommandInfos = new[]
            {
                QpCommandInfo.Create(new QpCommands.Login.Request()),
                QpCommandInfo.Create(new QpCommands.ListFolder.Request()),
                QpCommandInfo.Create(new QpCommands.CreateFolder.Request()),
                QpCommandInfo.Create(new QpCommands.MovePath.Request()),
                QpCommandInfo.Create(new QpCommands.Delete.Request()),
                QpCommandInfo.Create(new QpCommands.BeginUploadFile.Request()),
                QpCommandInfo.Create(new QpCommands.EndUploadFile.Request()),
                QpCommandInfo.Create(new QpCommands.BeginDownloadFile.Request()),
                QpCommandInfo.Create(new QpCommands.EndDownloadFile.Request())
            },
            NoticeInfos = new[]
            {
                QpNoticeInfo.Create(new QpNotices.FilePartInfo())
            }
        };
    }
}
