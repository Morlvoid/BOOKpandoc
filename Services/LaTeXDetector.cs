using System.Diagnostics;

namespace BOOKpandoc.Services
{
    public class LaTeXDetector
    {
        public bool IsLaTeXAvailable()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "xelatex",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processStartInfo };
                process.Start();
                process.WaitForExit(2000);

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}