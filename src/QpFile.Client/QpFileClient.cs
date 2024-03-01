using Quick.Protocol;
using System;
using System.Collections.Generic;
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
        public QpFileClient(QpFileClientOptions options)
        {
            this.options = options;
            clientOptions = QpClientOptions.Parse(new Uri(options.ConnectionString));
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

        public void Disconnect()
        {
            client?.Disconnect();
        }
    }
}
