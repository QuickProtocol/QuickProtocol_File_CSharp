
namespace QpFile.Server
{
    public class UserFolderInfo
    {
        public string Folder { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public FolderPermission Permission { get; set; }

        /// <summary>
        /// 路径是否有读权限
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool HasReadPermission(string path)
        {
            if (Folder == null) return false;
            if (Permission == FolderPermission.ReadOnly || Permission == FolderPermission.ReadAndWrite)
            {
                if (Folder == "*" || path.StartsWith(Folder))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 路径是否有写权限
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool HasWritePermission(string path)
        {
            if (Folder == null) return false;
            if (Permission == FolderPermission.WriteOnly || Permission == FolderPermission.ReadAndWrite)
            {
                if (Folder == "*" || path.StartsWith(Folder))
                    return true;
            }
            return false;
        }
    }
}
