using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QpFile.Server
{
    public class UserInfo
    {
        public string User { get; set; }
        public UserFolderInfo[] Folders { get; set; }
    }
}
