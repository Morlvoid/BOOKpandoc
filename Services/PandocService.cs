using System.Diagnostics;
using System.Text;
using System.IO;
using System;

namespace BOOKpandoc.Services
{
    public class PandocService
    {
        private readonly string _pandocPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pandoc.exe");
        private readonly LogService _logService = new LogService();
        private readonly List<string> _tempFiles = new List<string>();

        public bool IsPandocAvailable
        {
            get
            {
                // 首先检查当前目录是否有pandoc.exe
                if (File.Exists(_pandocPath))
                {
                    _logService.Info($"找到pandoc.exe: {_pandocPath}");
                    return true;
                }

                // 然后检查系统路径中是否有pandoc命令
                try
                {
                    var processStartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "pandoc",
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = new System.Diagnostics.Process { StartInfo = processStartInfo };
                    process.Start();
                    process.WaitForExit(2000);

                    var result = process.ExitCode == 0;
                    _logService.Info($"系统路径中pandoc可用: {result}");
                    return result;
                }
                catch (Exception ex)
                {
                    _logService.Error("检查pandoc可用性时出错", ex);
                    return false;
                }
            }
        }

        public async Task<string> ConvertAsync(List<string> chapterFiles, string outputPath, string format, string? theme, string title, string author, string? coverImagePath = null, string? referenceDocPath = null, CancellationToken cancellationToken = default)
        {
            _logService.Info("开始执行Pandoc转换");
            
            if (!IsPandocAvailable)
            {
                _logService.Error("Pandoc未找到");
                throw new Exception("您的电脑未安装Pandoc转换工具。\n\n请点击界面左侧的「下载Pandoc」按钮进行安装。\n\nPandoc是一个免费的文档转换工具，用于将Markdown文件转换为电子书格式。");
            }

            var args = new StringBuilder();

            // 添加章节文件
            foreach (var file in chapterFiles)
            {
                args.Append($"\"{file}\" ");
            }
            _logService.Info($"章节文件: {string.Join(", ", chapterFiles)}");

            // 添加输出格式
            args.Append($"-o \"{outputPath}\" ");
            _logService.Info($"输出路径: {outputPath}");

            // 添加元数据
            if (!string.IsNullOrEmpty(title))
            {
                args.Append($"--metadata title=\"{title}\" ");
                _logService.Info($"标题: {title}");
            }
            if (!string.IsNullOrEmpty(author))
            {
                args.Append($"--metadata author=\"{author}\" ");
                _logService.Info($"作者: {author}");
            }

            // 添加主题
            if (!string.IsNullOrEmpty(theme) && (format == "HTML" || format == "EPUB"))
            {
                // 尝试从项目根目录的themes文件夹加载主题文件
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // 对于开发环境，bin目录的上两级是项目根目录
                var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
                var themePath = Path.Combine(projectRoot, "themes", theme);
                
                _logService.Info($"尝试从项目根目录加载主题: {themePath}");
                if (!File.Exists(themePath))
                {
                    // 如果项目根目录不存在，则尝试从当前目录加载
                    themePath = Path.Combine(baseDir, "themes", theme);
                    _logService.Info($"尝试从当前目录加载主题: {themePath}");
                }
                
                if (File.Exists(themePath))
                {
                    // 统一使用 --css 参数，适用于所有支持CSS的格式
                    args.Append($"--css=\"{themePath}\" ");
                    _logService.Info($"已添加CSS主题: {theme}");
                }
                else
                {
                    _logService.Warning($"主题文件不存在: {themePath}");
                }
            }

            // 特殊处理 PDF
            if (format == "PDF")
            {
                args.Append("--pdf-engine=xelatex ");
                _logService.Info("添加PDF引擎参数: xelatex");
            }

            // 特殊处理 HTML，添加 --embed-resources --standalone 参数
            if (format.Equals("HTML", StringComparison.OrdinalIgnoreCase))
            {
                args.Append("--embed-resources --standalone ");
                _logService.Info("添加HTML自包含参数: --embed-resources --standalone");
            }

            // 处理封面图片
            if (!string.IsNullOrEmpty(coverImagePath) && File.Exists(coverImagePath))
            {
                if (format.Equals("EPUB", StringComparison.OrdinalIgnoreCase))
                {
                    args.Append($"--epub-cover-image=\"{coverImagePath}\" ");
                    _logService.Info($"添加EPUB封面图片: {coverImagePath}");
                }
                else if (format.Equals("HTML", StringComparison.OrdinalIgnoreCase))
                {
                    // 对于HTML，使用 --include-in-header 注入内联样式
                    try
                    {
                        // 将封面图片复制到输出目录
                        string? outputDir = Path.GetDirectoryName(outputPath);
                        if (!string.IsNullOrEmpty(outputDir))
                        {
                            string coverFileName = Path.GetFileName(coverImagePath);
                            string destCoverPath = Path.Combine(outputDir, coverFileName);
                            if (!File.Exists(destCoverPath))
                            {
                                File.Copy(coverImagePath, destCoverPath, true);
                            }
                            _logService.Info($"复制封面图片到输出目录: {destCoverPath}");

                            // 生成一个临时的 HTML 片段，包含封面背景样式
                            string tempHeaderFile = Path.Combine(Path.GetTempPath(), $"cover_{Guid.NewGuid()}.html");
                            string styleContent = $@"
<style>
#title-block-header {{
    background-image: url('{coverFileName}');
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
    padding: 2em;
    color: white;
    text-shadow: 1px 1px 2px black;
}}
</style>
";
                            File.WriteAllText(tempHeaderFile, styleContent);
                            _tempFiles.Add(tempHeaderFile);
                            args.Append($"--include-in-header=\"{tempHeaderFile}\" ");
                            _logService.Info($"添加HTML封面背景样式: {tempHeaderFile}");
                        }
                        else
                        {
                            _logService.Error("输出目录为空，跳过封面处理");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Error("处理HTML封面图片失败", ex);
                    }
                }
            }

            // 处理DOCX参考文档
            if (!string.IsNullOrEmpty(referenceDocPath) && File.Exists(referenceDocPath) && format.Equals("DOCX", StringComparison.OrdinalIgnoreCase))
            {
                args.Append($"--reference-doc=\"{referenceDocPath}\" ");
                _logService.Info($"添加DOCX参考文档: {referenceDocPath}");
            }

            // 选择pandoc执行文件
            string pandocExecutable = File.Exists(_pandocPath) ? _pandocPath : "pandoc";
            _logService.Info($"使用pandoc执行文件: {pandocExecutable}");

            // 设置工作目录为输出目录，确保Pandoc能正确找到图片文件
            string workingDirectory = Path.GetDirectoryName(outputPath) ?? Environment.CurrentDirectory;
            
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pandocExecutable,
                Arguments = args.ToString().Trim(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = workingDirectory
            };
            
            _logService.Info($"设置工作目录: {workingDirectory}");

            _logService.Info($"执行命令: {processStartInfo.FileName} {processStartInfo.Arguments}");

            // 使用 Task.Run 在后台线程执行进程，避免阻塞 UI 线程
            return await Task.Run(async () =>
            {
                try
                {
                    using var process = new Process { StartInfo = processStartInfo };
                    process.Start();

                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();

                    await Task.WhenAny(Task.WhenAll(outputTask, errorTask), Task.Delay(TimeSpan.FromMinutes(5), cancellationToken));

                    if (!process.HasExited)
                    {
                        process.Kill();
                        _logService.Error("转换超时，已终止进程");
                        throw new Exception("转换超时，请检查文件大小或尝试分批转换");
                    }

                    var error = await errorTask;
                    if (!string.IsNullOrEmpty(error))
                    {
                        _logService.Error($"转换失败: {error}");
                        throw new Exception($"转换失败: {error}");
                    }

                    var output = await outputTask;
                    _logService.Info("转换成功");
                    if (!string.IsNullOrEmpty(output))
                    {
                        _logService.Info($"转换输出: {output}");
                    }

                    return output;
                }
                finally
                {
                    // 清理临时文件
                    CleanupTempFiles();
                }
            }, cancellationToken);
        }

        private void CleanupTempFiles()
        {
            foreach (var tempFile in _tempFiles)
            {
                try
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                        _logService.Info($"已删除临时文件: {tempFile}");
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error($"删除临时文件失败: {tempFile}", ex);
                }
            }
            _tempFiles.Clear();
        }
    }
}