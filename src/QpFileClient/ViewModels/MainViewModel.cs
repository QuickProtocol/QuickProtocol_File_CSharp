namespace QpFileClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string ConnectionString { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string LocalPath { get; set; }
    public string RemotePath { get; set; }
    public MainViewModel()
    {
#if DEBUG
        ConnectionString = "qp.tcp://127.0.0.1:16421?Password=QpFile";
        User = "test";
        Password = "test";
#endif
    }

    public void Connect()
    {

    }
}
