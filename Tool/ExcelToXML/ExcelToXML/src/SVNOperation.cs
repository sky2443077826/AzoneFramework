using System.Diagnostics;

public static class SVNOperation
{
    /// <summary>
    /// 创建一个SVN的cmd命令
    /// </summary>
    /// <param name="command">命令(可在help里边查看)</param>
    /// <param name="path">命令激活路径</param>
    public static void SVNCommand(string command, string path)
    {
        //TortoiseSVN/help/TortoiseSVN/Automating TortoiseSVN里查看
        string c = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" /closeonend 0";
        c = string.Format(c, command, path);
        ProcessStartInfo info = new ProcessStartInfo("cmd.exe", c);
        info.WindowStyle = ProcessWindowStyle.Hidden;
        Process.Start(info);
    }

}
