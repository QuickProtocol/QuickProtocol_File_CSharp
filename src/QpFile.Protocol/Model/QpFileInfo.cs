using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Protocol.Model
{
    public class QpFileInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 最后写入时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }
    }
}
