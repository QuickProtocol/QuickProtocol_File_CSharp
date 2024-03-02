using QpFile.Protocol.Model;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Client
{
    public class QpFileClient
    {
        private QpFileClientOptions options;
        private QpClientOptions clientOptions;
        private QpClient client;

        private CommandExecuterManager commandExecuterManager;
        private NoticeHandlerManager noticeHandlerManager;
        private Dictionary<string, FileTransferInfo> downloadFileDict = new Dictionary<string, FileTransferInfo>();

        public QpFileClient(QpFileClientOptions options)
        {
            this.options = options;
            clientOptions = QpClientOptions.Parse(new Uri(options.ConnectionString));

            commandExecuterManager = new CommandExecuterManager();
            commandExecuterManager.Register<Protocol.QpCommands.EndDownloadFile.Request, Protocol.QpCommands.EndDownloadFile.Response>(executeCommand_EndDownloadFile);
            noticeHandlerManager = new NoticeHandlerManager();
            noticeHandlerManager.Register<Protocol.QpNotices.FilePartInfo>(OnFilePartInfoNotice);
        }

        public async Task ConnectAsync()
        {
            client = clientOptions.CreateClient();
            await client.ConnectAsync();
            await client.SendCommand(new Protocol.QpCommands.Login.Request()
            {
                User = options.User,
                Password = options.Password
            });
        }

        public async Task<Protocol.QpCommands.ListFolder.Response> ListFolder(string folder)
        {
            return await client.SendCommand(new Protocol.QpCommands.ListFolder.Request()
            {
                Folder = folder
            });
        }

        public async Task CreateFolder(string folder)
        {
            await client.SendCommand(new Protocol.QpCommands.CreateFolder.Request()
            {
                Folder = folder
            });
        }

        public async Task MovePath(string sourcePath, string targetPath)
        {
            await client.SendCommand(new Protocol.QpCommands.MovePath.Request()
            {
                SourcePath = sourcePath,
                TargetPath = targetPath
            });
        }

        public async Task Delete(string path)
        {
            await client.SendCommand(new Protocol.QpCommands.Delete.Request()
            {
                Path = path
            });
        }

        public async Task UploadFile(string file, Action<FileTransferProgress> transferProgressHandler = null)
        {
            await UploadFile(file, CancellationToken.None, transferProgressHandler);
        }

        public async Task UploadFile(string file, CancellationToken cancellationToken, Action<FileTransferProgress> transferProgressHandler = null)
        {
            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
                throw new IOException($"File[{file}] not exist.");
            var fileLength = fileInfo.Length;
            //发送文件上传开始指令
            var rep = await client.SendCommand(new Protocol.QpCommands.BeginUploadFile.Request()
            {
                Path = fileInfo.FullName,
                FileLength = fileLength
            });
            var fileId = rep.FileId;
            var bufferSize = options.TransferBufferSize;
            var buffer = new byte[bufferSize];
            using (var fs = fileInfo.OpenRead())
            {
                long offset = 0;
                try
                {
                    while (true)
                    {
                        var ret = await fs.ReadAsync(buffer, 0, bufferSize, cancellationToken);
                        if (ret <= 0)
                            break;
                        //发送文件内容
                        await client.SendNoticePackage(new Protocol.QpNotices.FilePartInfo()
                        {
                            FileId = fileId,
                            FileData = ret == bufferSize ? buffer : buffer.Take(ret).ToArray(),
                            Length = ret,
                            Offset = offset
                        });
                        offset += ret;
                        //通知进度
                        transferProgressHandler?.Invoke(new FileTransferProgress()
                        {
                            Offset = offset,
                            Length = fileLength
                        });
                    }
                    //发送文件上传结束指令
                    await client.SendCommand(new Protocol.QpCommands.EndUploadFile.Request()
                    {
                        FileId = fileId,
                        LastWriteTime = fileInfo.LastWriteTime
                    });
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public async Task DownloadFile(
            string remotePath, string localFile,
            Action<FileTransferProgress> transferProgressHandler = null, int timeout = 10000)
        {
            await DownloadFile(remotePath, localFile, CancellationToken.None, transferProgressHandler, timeout);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="localFile"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="transferProgressHandler"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task DownloadFile(
            string remotePath, string localFile, CancellationToken cancellationToken,
            Action<FileTransferProgress> transferProgressHandler = null, int timeout = 10000)
        {
            var fs = File.Create(localFile);
            //发送文件下载开始指令
            var rep = await client.SendCommand(new Protocol.QpCommands.BeginDownloadFile.Request()
            {
                Path = remotePath
            });
            var fileLength = rep.Length;
            var fileId = rep.FileId;
            var fileTransferInfo = new FileTransferInfo(fileId, localFile, fileLength, fs);
            lock (downloadFileDict)
            {
                downloadFileDict[fileId] = fileTransferInfo;
            }
            while (true)
            {
                try
                {
                    await Task.Delay(1000, cancellationToken);
                    //判断文件传输是否超时
                    if ((DateTime.Now - fileTransferInfo.LastTransferTime).TotalMilliseconds > timeout)
                    {
                        fileTransferInfo.SetErrorMessage($"File transfer timeout.");
                        fileTransferInfo.Dispose();
                    }
                    //判断文件传输是否已经结束
                    if (fileTransferInfo.IsCompleted)
                    {
                        //移除
                        lock (downloadFileDict)
                            if (downloadFileDict.ContainsKey(fileId))
                                downloadFileDict.Remove(fileId);

                        //文件传输是否存在错误
                        if (!string.IsNullOrEmpty(fileTransferInfo.ErrorMessage))
                        {
                            if (File.Exists(fileTransferInfo.Path))
                                File.Delete(fileTransferInfo.Path);
                            throw new IOException(fileTransferInfo.ErrorMessage);
                        }
                        break;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    //移除
                    lock (downloadFileDict)
                        if (downloadFileDict.ContainsKey(fileId))
                            downloadFileDict.Remove(fileId);
                    fileTransferInfo.SetErrorMessage(ex.Message);
                    fileTransferInfo.Dispose();
                    throw;
                }
            }
        }

        private void OnFilePartInfoNotice(QpChannel channel, Protocol.QpNotices.FilePartInfo info)
        {
            var fileId = info.FileId;
            if (!downloadFileDict.TryGetValue(fileId, out var fileTransferInfo))
                return;
            //验证Offset
            if (fileTransferInfo.LastOffset != info.Offset)
            {
                fileTransferInfo.SetErrorMessage($"File part[Id:{fileId},Path:{fileTransferInfo.Path}] offset not match.Server offset:{fileTransferInfo.LastOffset},Client offset:{info.Offset}");
                fileTransferInfo.Dispose();
                return;
            }

            //验证Length
            if (info.Length != info.FileData.Length)
            {
                fileTransferInfo.SetErrorMessage($"File part[Id:{fileId},Path:{fileTransferInfo.Path}] length not match.Length:{info.Length},FileData.Length:{info.FileData.Length}");
                fileTransferInfo.Dispose();
                return;
            }
            fileTransferInfo.Write(info.FileData);
        }

        private Protocol.QpCommands.EndDownloadFile.Response executeCommand_EndDownloadFile(QpChannel channel, Protocol.QpCommands.EndDownloadFile.Request request)
        {
            var fileId = request.FileId;
            if (downloadFileDict.TryGetValue(fileId, out var fileTransferInfo))
            {
                if (fileTransferInfo.LastOffset != fileTransferInfo.Length)
                    fileTransferInfo.SetErrorMessage($"File[Id:{fileId},Path:{fileTransferInfo.Path}] transfer last offset[{fileTransferInfo.LastOffset}] not match length[{fileTransferInfo.Length}]");
                fileTransferInfo.Dispose();
            }
            return new Protocol.QpCommands.EndDownloadFile.Response();
        }

        public void Disconnect()
        {
            client?.Disconnect();
        }
    }
}
