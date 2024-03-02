using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Server
{
    public class QpFileServerOptions
    {
        public QpServerOptions QpServerOptions { get; set; }
        /// <summary>
        /// 登录函数
        /// </summary>
        public Func<string, string, UserInfo> LoginFunc { get; set; }
        /// <summary>
        /// 传输缓存大小
        /// </summary>
        public int TransferBufferSize { get; set; } = 4096;
    }
}
