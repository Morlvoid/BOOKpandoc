using System.Diagnostics;

namespace BOOKpandoc.Services
{
    public static class RsvgDetector
    {
        public static bool IsAvailable()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "rsvg-convert",
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(3000);
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public static string GetInstallGuide()
        {
            return "导出 DOCX 需要处理 SVG 图片，但系统中未找到 rsvg-convert。\n\n" +
                   "请安装 librsvg 工具，例如：\n" +
                   "• 下载 Windows 版本： `https://github.com/gtk-rs/librsvg/releases\n` " +
                   "• 或通过 MSYS2 安装：pacman -S mingw-w64-x86_64-librsvg\n\n" +
                   "安装后请确保 rsvg-convert.exe 所在目录已添加到 PATH 环境变量。";
        }
    }
}
