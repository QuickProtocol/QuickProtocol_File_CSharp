using QpFile.Protocol.Model;
using Quick.Protocol;
using System.IO;

namespace QpFile.Server
{
    public class QpFileServer
    {
        private QpFileServerOptions options;
        private CommandExecuterManager commandExecuterManager;
        private NoticeHandlerManager noticeHandlerManager;
        private Dictionary<string, FileTransferInfo> uploadFileDict = new Dictionary<string, FileTransferInfo>();

        public QpFileServer(QpFileServerOptions options)
        {
            this.options = options;
            commandExecuterManager = new CommandExecuterManager();
            commandExecuterManager.Register<Protocol.QpCommands.Login.Request, Protocol.QpCommands.Login.Response>(executeCommand_Login);
            commandExecuterManager.Register<Protocol.QpCommands.ListFolder.Request, Protocol.QpCommands.ListFolder.Response>(executeCommand_ListFolder);
            commandExecuterManager.Register<Protocol.QpCommands.CreateFolder.Request, Protocol.QpCommands.CreateFolder.Response>(executeCommand_CreateFolder);
            commandExecuterManager.Register<Protocol.QpCommands.MovePath.Request, Protocol.QpCommands.MovePath.Response>(executeCommand_MovePath);
            commandExecuterManager.Register<Protocol.QpCommands.Delete.Request, Protocol.QpCommands.Delete.Response>(executeCommand_Delete);
            commandExecuterManager.Register<Protocol.QpCommands.BeginUploadFile.Request, Protocol.QpCommands.BeginUploadFile.Response>(executeCommand_BeginUploadFile);
            commandExecuterManager.Register<Protocol.QpCommands.EndUploadFile.Request, Protocol.QpCommands.EndUploadFile.Response>(executeCommand_EndUploadFile);
            commandExecuterManager.Register<Protocol.QpCommands.BeginDownloadFile.Request, Protocol.QpCommands.BeginDownloadFile.Response>(executeCommand_BeginDownloadFile);
            noticeHandlerManager = new NoticeHandlerManager();
            noticeHandlerManager.Register<Protocol.QpNotices.FilePartInfo>(OnFilePartInfoNotice);

            options.QpServerOptions.RegisterCommandExecuterManager(commandExecuterManager);
            options.QpServerOptions.RegisterNoticeHandlerManager(noticeHandlerManager);
        }

        private Protocol.QpCommands.Login.Response executeCommand_Login(QpChannel channel, Protocol.QpCommands.Login.Request request)
        {
            var userInfo = options.LoginFunc(request.User, request.Password);
            channel.Tag = userInfo;
            return new Protocol.QpCommands.Login.Response();
        }

        private UserInfo getUserInfo(QpChannel channel)
        {
            var userInfo = channel.Tag as UserInfo;
            if (userInfo == null)
                throw new ApplicationException("Not login.");
            return userInfo;
        }

        private void checkReadPermission(QpChannel channel, string path)
        {
            var userInfo = getUserInfo(channel);
            var hasPermission = false;
            if (userInfo.Folders != null)
                hasPermission = userInfo.Folders.Any(t => t.HasReadPermission(path));
            if (!hasPermission)
                throw new ApplicationException("No permission to read from path: " + path);
        }

        private void checkWritePermission(QpChannel channel, string path)
        {
            var userInfo = getUserInfo(channel);
            var hasPermission = false;
            if (userInfo.Folders != null)
                hasPermission = userInfo.Folders.Any(t => t.HasWritePermission(path));
            if (!hasPermission)
                throw new ApplicationException("No permission to write from path: " + path);
        }

        private Protocol.QpCommands.ListFolder.Response executeCommand_ListFolder(QpChannel channel, Protocol.QpCommands.ListFolder.Request request)
        {
            var userInfo = getUserInfo(channel);
            if (userInfo.Folders == null)
                return new Protocol.QpCommands.ListFolder.Response();
            if (string.IsNullOrEmpty(request.Folder))
                return new Protocol.QpCommands.ListFolder.Response()
                {
                    Folders = userInfo.Folders
                    .Select(t => new DirectoryInfo(t.Folder))
                    .Where(t => t.Exists)
                    .Select(t => new QpFolderInfo()
                    {
                        Name = t.Name,
                        FullName = t.FullName,
                        LastWriteTime = t.LastWriteTime
                    }).ToArray()
                };
            var folder = Path.GetFullPath(request.Folder);
            checkReadPermission(channel, folder);
            if (!Directory.Exists(folder))
                throw new IOException($"Folder[{folder}] not exist.");
            return new Protocol.QpCommands.ListFolder.Response()
            {
                Folders = Directory.GetDirectories(folder)
                    .Select(t => new DirectoryInfo(t))
                    .Where(t => t.Exists)
                    .Select(t => new QpFolderInfo()
                    {
                        Name = t.Name,
                        FullName = t.FullName,
                        LastWriteTime = t.LastWriteTime
                    }).ToArray(),
                Files = Directory.GetFiles(folder)
                    .Select(t => new FileInfo(t))
                    .Where(t => t.Exists)
                    .Select(t => new QpFileInfo()
                    {
                        Name = t.Name,
                        FullName = t.FullName,
                        LastWriteTime = t.LastWriteTime,
                        Length = t.Length
                    }).ToArray()
            };
        }

        private Protocol.QpCommands.CreateFolder.Response executeCommand_CreateFolder(QpChannel channel, Protocol.QpCommands.CreateFolder.Request request)
        {
            var folder = Path.GetFullPath(request.Folder);
            checkWritePermission(channel, folder);
            Directory.CreateDirectory(folder);
            return new Protocol.QpCommands.CreateFolder.Response();
        }

        private Protocol.QpCommands.MovePath.Response executeCommand_MovePath(QpChannel channel, Protocol.QpCommands.MovePath.Request request)
        {
            var sourcePath = Path.GetFullPath(request.SourcePath);
            var targetPath = Path.GetFullPath(request.TargetPath);
            checkWritePermission(channel, sourcePath);
            checkWritePermission(channel, targetPath);
            if (Directory.Exists(sourcePath))
                Directory.Move(sourcePath, targetPath);
            else if (File.Exists(sourcePath))
                File.Move(sourcePath, targetPath);
            else
                throw new IOException($"Path[{sourcePath}] not exist.");
            return new Protocol.QpCommands.MovePath.Response();
        }

        private Protocol.QpCommands.Delete.Response executeCommand_Delete(QpChannel channel, Protocol.QpCommands.Delete.Request request)
        {
            var path = Path.GetFullPath(request.Path);
            checkWritePermission(channel, path);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            else if (File.Exists(path))
                File.Delete(path);
            else
                throw new ApplicationException($"Path[{path}] not exist.");
            return new Protocol.QpCommands.Delete.Response();
        }

        private Protocol.QpCommands.BeginUploadFile.Response executeCommand_BeginUploadFile(QpChannel channel, Protocol.QpCommands.BeginUploadFile.Request request)
        {
            var path = Path.GetFullPath(request.Path);
            checkWritePermission(channel, path);

            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                throw new IOException($"Folder[{folder}] not exist.");
            var fileId = Guid.NewGuid().ToString("N");
            FileTransferInfo fileTransferInfo = null;
            lock (uploadFileDict)
            {
                var fs = File.OpenWrite(path);
                fileTransferInfo = new FileTransferInfo(fileId, path, request.FileLength, fs);
                uploadFileDict[fileId] = fileTransferInfo;
            }
            Task.Run(async () =>
            {
                while (!fileTransferInfo.IsDisposed)
                {
                    await Task.Delay(1000);
                    //如果检测到上传文件持续1分钟都没有收到文件数据，则删除文件
                    if ((DateTime.Now - fileTransferInfo.LastTransferTime).TotalMinutes > 1)
                    {
                        lock (uploadFileDict)
                        {
                            fileTransferInfo.Dispose();
                            if (uploadFileDict.ContainsKey(fileId))
                                uploadFileDict.Remove(fileId);
                        }
                        File.Delete(path);
                    }
                }
            });
            return new Protocol.QpCommands.BeginUploadFile.Response() { FileId = fileId };
        }

        private Protocol.QpCommands.EndUploadFile.Response executeCommand_EndUploadFile(QpChannel channel, Protocol.QpCommands.EndUploadFile.Request request)
        {
            var fileId = request.FileId;
            lock (uploadFileDict)
            {
                if (!uploadFileDict.TryGetValue(fileId, out var fileTransferInfo))
                    throw new IOException($"File[Id:{fileId}] not exist.");
                fileTransferInfo.Dispose();
                if (fileTransferInfo.LastOffset != fileTransferInfo.Length)
                {
                    File.Delete(fileTransferInfo.Path);
                    throw new IOException($"File[Id:{fileId},Path:{fileTransferInfo.Path}] broken.");
                }
                File.SetLastWriteTime(fileTransferInfo.Path, request.LastWriteTime);
            }
            return new Protocol.QpCommands.EndUploadFile.Response();
        }

        private Protocol.QpCommands.BeginDownloadFile.Response executeCommand_BeginDownloadFile(QpChannel channel, Protocol.QpCommands.BeginDownloadFile.Request request)
        {
            var path = Path.GetFullPath(request.Path);
            checkReadPermission(channel, path);
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                throw new IOException($"File[{path}] not exist.");
            var fs = fileInfo.OpenRead();
            var fileId = Guid.NewGuid().ToString("N");
            //等待2秒后，开始发送文件数据
            Task.Delay(2000).ContinueWith(async t =>
            {
                var bufferSize = 4096;
                var buffer = new byte[bufferSize];
                using (fs)
                {
                    long offset = 0;
                    while (true)
                    {
                        var ret = fs.Read(buffer, 0, buffer.Length);
                        if (ret <= 0)
                            break;
                        //发送文件内容
                        await channel.SendNoticePackage(new Protocol.QpNotices.FilePartInfo()
                        {
                            FileId = fileId,
                            FileData = ret == bufferSize ? buffer : buffer.Take(ret).ToArray(),
                            Length = ret,
                            Offset = offset
                        });
                        offset += ret;
                    }
                    //发送结束下载指令
                    await channel.SendCommand(new Protocol.QpCommands.EndDownloadFile.Request()
                    {
                        FileId = fileId,
                        LastWriteTime = fileInfo.LastWriteTime
                    });
                }
            });
            return new Protocol.QpCommands.BeginDownloadFile.Response()
            {
                FileId = fileId,
                Length = fileInfo.Length
            };
        }

        private void OnFilePartInfoNotice(QpChannel channel, Protocol.QpNotices.FilePartInfo info)
        {
            var fileId = info.FileId;
            if (!uploadFileDict.TryGetValue(fileId, out var fileTransferInfo))
                return;
            //验证Offset
            if (fileTransferInfo.LastOffset != info.Offset)
                throw new IOException($"File part[Id:{fileId},Path:{fileTransferInfo.Path}] offset not match.Server offset:{fileTransferInfo.LastOffset},Client offset:{info.Offset}");
            //验证Length
            if (info.Length != info.FileData.Length)
                throw new IOException($"File part[Id:{fileId},Path:{fileTransferInfo.Path}] length not match.Length:{info.Length},FileData.Length:{info.FileData.Length}");
            fileTransferInfo.Write(info.FileData);
        }
    }
}
