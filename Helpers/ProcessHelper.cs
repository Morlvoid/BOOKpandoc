using System.Diagnostics;

namespace BOOKpandoc.Helpers
{
    public static class ProcessHelper
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                // 忽略错误
            }
        }

        public static void OpenFolder(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = true
                });
            }
            catch
            {
                // 忽略错误
            }
        }
    }
}