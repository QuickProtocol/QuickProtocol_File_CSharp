using QpFile.Server;
using Quick.Protocol.Tcp;

var qpServerOptions = new QpTcpServerOptions()
{
    Address = System.Net.IPAddress.Loopback,
    Port = 16421,
    Password = nameof(QpFile),
    ServerProgram = "QpFileServer_Tcp"
};
var qpFileServer = new QpFileServer(new QpFileServerOptions()
{
    LoginFunc = (user, password) => new UserInfo()
    {
        User = user,
        Folders = new[] { new UserFolderInfo() { Folder = "*", Permission = FolderPermission.ReadAndWrite } }
    },
    QpServerOptions = qpServerOptions
});
var qpServer = qpServerOptions.CreateServer();
qpServer.Start();
Console.WriteLine($"启动完成，连接字符串：qp.tcp://{qpServerOptions.Address}:{qpServerOptions.Port}?Password={nameof(QpFile)}");
Console.ReadLine();
