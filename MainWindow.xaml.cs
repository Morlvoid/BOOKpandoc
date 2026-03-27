using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BOOKpandoc.Models;
using BOOKpandoc.Services;
using BOOKpandoc.Helpers;
using WinForms = System.Windows.Forms;

namespace BOOKpandoc;

public partial class MainWindow : System.Windows.Window
{
    private ObservableCollection<Chapter> _chapters = new ObservableCollection<Chapter>();
    private PandocService _pandocService = new PandocService();
    private ThemeService _themeService = new ThemeService();
    private LaTeXDetector _latexDetector = new LaTeXDetector();
    private AppSettings _settings = AppSettings.Load();
    private LogService _logService = new LogService();
    private string _coverImagePath = string.Empty;
    private string _referenceDocPath = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        _logService.Info("应用程序启动");
        InitializeUI();
        InitializeWindowButtons();
        ChapterListBox.ItemsSource = _chapters;
        LoadSettings();
        RefreshThemes();
        _logService.Info("UI初始化完成");
    }

    private void InitializeUI()
    {
        FormatComboBox.SelectedIndex = 1; // 默认选择EPUB
        StatusTextBlock.Text = "就绪";
    }

    private void LoadSettings()
    {
        if (!string.IsNullOrEmpty(_settings.OutputDirectory))
        {
            OutputDirectoryTextBox.Text = _settings.OutputDirectory;
            _logService.Info($"加载上次输出目录: {_settings.OutputDirectory}");
        }
    }

    private void InitializeWindowButtons()
    {
        // 窗口按钮事件已经在XAML中定义
    }

    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // 双击标题栏最大化/还原窗口
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        else
        {
            DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _logService.Info("应用程序关闭");
        Close();
    }

    private async void DownloadPandocButton_Click(object sender, RoutedEventArgs e)
    {
        _logService.Info("开始检测并安装Pandoc");
        StatusTextBlock.Text = "正在检测Pandoc...";
        
        try
        {
            // 检测Pandoc是否已安装
            if (IsPandocInstalled())
            {
                _logService.Info("Pandoc已安装");
                StatusTextBlock.Text = "Pandoc已安装";
                System.Windows.MessageBox.Show(
                    "Pandoc已经安装好了！\n\n您可以开始使用本软件生成电子书了。", 
                    "Pandoc已安装", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                return;
            }
            
            // 尝试使用winget安装Pandoc
            StatusTextBlock.Text = "正在使用winget安装Pandoc...";
            _logService.Info("开始使用winget安装Pandoc");
            
            var result = await RunWingetInstallAsync();
            
            if (result)
            {
                _logService.Info("Pandoc安装成功");
                StatusTextBlock.Text = "Pandoc安装成功";
                System.Windows.MessageBox.Show(
                    "Pandoc安装成功！\n\n您现在可以开始使用本软件生成电子书了。", 
                    "安装成功", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
            else
            {
                _logService.Info("winget安装失败，打开下载页面");
                StatusTextBlock.Text = "winget安装失败，打开下载页面";
                
                // 尝试打开官方下载页面
                ProcessHelper.OpenUrl("https://pandoc.org/installing.html");
                System.Windows.MessageBox.Show(
                    "winget安装失败，已为您打开Pandoc官方下载页面。\n\n请按照页面提示手动下载并安装Pandoc。\n\n安装完成后，您就可以开始使用本软件生成电子书了。", 
                    "安装失败", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            _logService.Error("安装Pandoc失败", ex);
            StatusTextBlock.Text = "安装Pandoc失败";
            System.Windows.MessageBox.Show(
                $"安装Pandoc时发生错误。\n\n您可以手动访问以下网址下载：\nhttps://pandoc.org/installing.html\n\n错误详情：{ex.Message}", 
                "安装失败", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
    }

    private bool IsPandocInstalled()
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "pandoc",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.WaitForExit(3000); // 3秒超时
            
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private async System.Threading.Tasks.Task<bool> RunWingetInstallAsync()
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = "install --source winget --exact --id JohnMacFarlane.Pandoc",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            
            // 异步等待进程完成
            await System.Threading.Tasks.Task.Run(() => process.WaitForExit());
            
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new WinForms.OpenFileDialog
        {
            Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            foreach (var file in dialog.FileNames)
            {
                _chapters.Add(new Chapter(file));
                _logService.Info($"添加文件: {file}");
            }
        }
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (ChapterListBox.SelectedItem is Chapter chapter)
        {
            _chapters.Remove(chapter);
            _logService.Info($"移除文件: {chapter.FilePath}");
        }
    }

    private void MoveUpButton_Click(object sender, RoutedEventArgs e)
    {
        if (ChapterListBox.SelectedItem is Chapter chapter)
        {
            int index = _chapters.IndexOf(chapter);
            if (index > 0)
            {
                _chapters.Move(index, index - 1);
                _logService.Info($"文件上移: {chapter.FileName}");
            }
        }
    }

    private void MoveDownButton_Click(object sender, RoutedEventArgs e)
    {
        if (ChapterListBox.SelectedItem is Chapter chapter)
        {
            int index = _chapters.IndexOf(chapter);
            if (index < _chapters.Count - 1)
            {
                _chapters.Move(index, index + 1);
                _logService.Info($"文件下移: {chapter.FileName}");
            }
        }
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new WinForms.FolderBrowserDialog();
        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            OutputDirectoryTextBox.Text = dialog.SelectedPath;
            _settings.OutputDirectory = dialog.SelectedPath;
            _settings.Save();
            _logService.Info($"设置输出目录: {dialog.SelectedPath}");
        }
    }

    private void RefreshThemesButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshThemes();
    }

    private void RefreshThemes()
    {
        var themes = _themeService.GetAvailableThemes();
        ThemeComboBox.ItemsSource = themes;
        if (themes.Any())
        {
            ThemeComboBox.SelectedIndex = 0;
        }
        _logService.Info($"刷新主题列表，找到 {themes.Count} 个主题");
    }

    private void SelectCoverImageButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new WinForms.OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            _coverImagePath = dialog.FileName;
            CoverImagePathTextBlock.Text = System.IO.Path.GetFileName(_coverImagePath);
            _logService.Info($"选择封面图片: {_coverImagePath}");
        }
    }

    private void ClearCoverImageButton_Click(object sender, RoutedEventArgs e)
    {
        _coverImagePath = string.Empty;
        CoverImagePathTextBlock.Text = "未选择";
        _logService.Info("清除封面图片");
    }

    private void SelectReferenceDocButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new WinForms.OpenFileDialog
        {
            Filter = "Word documents (*.docx)|*.docx|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            _referenceDocPath = dialog.FileName;
            ReferenceDocPathTextBlock.Text = System.IO.Path.GetFileName(_referenceDocPath);
            _logService.Info($"选择Word模板: {_referenceDocPath}");
        }
    }

    private void ClearReferenceDocButton_Click(object sender, RoutedEventArgs e)
    {
        _referenceDocPath = string.Empty;
        ReferenceDocPathTextBlock.Text = "未选择";
        _logService.Info("清除Word模板");
    }

    private async void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_chapters.Count == 0)
        {
            System.Windows.MessageBox.Show(
                "您还没有添加任何Markdown文件。\n\n请点击「添加文件」按钮选择要转换的Markdown文件。", 
                "提示", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrEmpty(OutputDirectoryTextBox.Text))
        {
            System.Windows.MessageBox.Show(
                "您还没有选择输出目录。\n\n请点击「浏览」按钮选择电子书的保存位置。", 
                "提示", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            return;
        }

        var format = GetSelectedFormat();
        var theme = ThemeComboBox.SelectedItem?.ToString();
        var outputPath = System.IO.Path.Combine(OutputDirectoryTextBox.Text, $"output.{format.ToLower()}");

        StatusTextBlock.Text = "正在生成电子书...";
        GenerateButton.IsEnabled = false;

        try
        {
            var chapterFiles = _chapters.Select(c => c.FilePath).ToList();
            var result = await _pandocService.ConvertAsync(
                chapterFiles,
                outputPath,
                format,
                theme,
                "",
                "",
                _coverImagePath,
                _referenceDocPath
            );

            _logService.Info($"生成成功: {result}");
            StatusTextBlock.Text = "生成完成！";
            System.Windows.MessageBox.Show(
                $"恭喜！电子书生成成功！\n\n文件已保存到：\n{result}\n\n您现在可以用相应的阅读器打开这个文件了。", 
                "生成成功", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            _logService.Error("生成失败", ex);
            StatusTextBlock.Text = "生成失败";
            System.Windows.MessageBox.Show(
                $"电子书生成失败。\n\n错误信息：\n{ex.Message}\n\n建议：\n1. 检查是否已安装Pandoc\n2. 确保所有Markdown文件存在\n3. 检查输出目录是否有写入权限", 
                "生成失败", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
        finally
        {
            GenerateButton.IsEnabled = true;
        }
    }

    private string GetSelectedFormat()
    {
        return FormatComboBox.SelectedIndex switch
        {
            0 => "PDF",
            1 => "EPUB",
            2 => "DOCX",
            3 => "HTML",
            _ => "EPUB"
        };
    }

    private void ExampleButton_Click(object sender, RoutedEventArgs e)
    {
        // 打开示例文件夹或显示示例说明
        System.Windows.MessageBox.Show("示例功能开发中...", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // 拖拽添加文件 + 内部排序
    private void ChapterListBox_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
    {
        // 检查是否拖入文件或内部项
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }

    private void ChapterListBox_Drop(object sender, System.Windows.DragEventArgs e)
    {
        // 处理文件拖拽
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                if (file.EndsWith(".md", System.StringComparison.OrdinalIgnoreCase))
                {
                    _chapters.Add(new Chapter(file));
                }
                else if (file.EndsWith(".css", System.StringComparison.OrdinalIgnoreCase))
                {
                    _themeService.ImportTheme(file);
                    RefreshThemes();
                    ThemeComboBox.SelectedItem = System.IO.Path.GetFileName(file);
                }
            }
            return;
        }

        // 处理内部拖拽排序
        var sourceItem = e.Data.GetData(typeof(Chapter)) as Chapter;
        if (sourceItem != null)
        {
            var targetItem = GetTargetItemFromDrop(sender as System.Windows.Controls.ListBox, e.GetPosition(sender as System.Windows.Controls.ListBox));
            if (targetItem != null)
            {
                int sourceIndex = _chapters.IndexOf(sourceItem);
                int targetIndex = _chapters.IndexOf(targetItem);

                if (sourceIndex != targetIndex)
                {
                    _chapters.Move(sourceIndex, targetIndex);
                }
            }
        }
    }

    // 辅助方法：获取鼠标下的列表项
    private Chapter? GetTargetItemFromDrop(System.Windows.Controls.ListBox? listBox, System.Windows.Point point)
    {
        if (listBox == null) return null;
        
        var result = VisualTreeHelper.HitTest(listBox, point);
        while (result != null)
        {
            if (result.VisualHit is FrameworkElement element && element.DataContext is Chapter chapter)
                return chapter;
            var parent = VisualTreeHelper.GetParent(result.VisualHit);
            if (parent != null)
            {
                result = VisualTreeHelper.HitTest((Visual)parent, point);
            }
            else
            {
                break;
            }
        }
        return null;
    }
}
