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
        /// 是否有读权限
        /// </summary>
        public bool HasReadPermission => Permission == FolderPermission.ReadOnly || Permission == FolderPermission.ReadAndWrite;
        /// <summary>
        /// 是否有写权限
        /// </summary>
        public bool HasWritePermission => Permission == FolderPermission.WriteOnly || Permission == FolderPermission.ReadAndWrite;
    }
}
