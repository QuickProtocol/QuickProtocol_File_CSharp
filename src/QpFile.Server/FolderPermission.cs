using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Server
{
    public enum FolderPermission
    {
        /// <summary>
        /// 只读
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// 只写
        /// </summary>
        WriteOnly = 2,
        /// <summary>
        /// 读写
        /// </summary>
        ReadAndWrite = 3
    }
}
