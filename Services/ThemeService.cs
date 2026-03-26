using System.IO;

namespace BOOKpandoc.Services
{
    public class ThemeService
    {
        private readonly string _themesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes");
        private readonly LogService _logService = new LogService();

        public ThemeService()
        {
            try
            {
                if (!Directory.Exists(_themesDirectory))
                {
                    Directory.CreateDirectory(_themesDirectory);
                    _logService.Info($"创建主题目录: {_themesDirectory}");
                }
            }
            catch (Exception ex)
            {
                _logService.Error("创建主题目录失败", ex);
            }
        }

        public List<string> GetAvailableThemes()
        {
            var themes = new List<string>();

            if (!Directory.Exists(_themesDirectory))
            {
                _logService.Warning($"主题目录不存在: {_themesDirectory}");
                themes.Add("default.css");
                return themes;
            }

            try
            {
                var cssFiles = Directory.GetFiles(_themesDirectory, "*.css");
                foreach (var file in cssFiles)
                {
                    themes.Add(Path.GetFileName(file));
                }
                _logService.Info($"找到 {themes.Count} 个主题文件");
            }
            catch (Exception ex)
            {
                _logService.Error("获取主题列表失败", ex);
            }

            if (themes.Count == 0)
            {
                themes.Add("default.css");
                _logService.Info("没有找到主题文件，使用默认主题");
            }

            return themes;
        }

        public bool ImportTheme(string cssFilePath)
        {
            if (!File.Exists(cssFilePath))
            {
                _logService.Warning($"要导入的主题文件不存在: {cssFilePath}");
                return false;
            }

            try
            {
                var fileName = Path.GetFileName(cssFilePath);
                var destPath = Path.Combine(_themesDirectory, fileName);
                
                // 确保目标目录存在
                if (!Directory.Exists(_themesDirectory))
                {
                    Directory.CreateDirectory(_themesDirectory);
                }
                
                File.Copy(cssFilePath, destPath, true);
                _logService.Info($"成功导入主题: {fileName} 到 {destPath}");
                return true;
            }
            catch (Exception ex)
            {
                _logService.Error($"导入主题失败: {cssFilePath}", ex);
                return false;
            }
        }

        public string GetThemePath(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                return string.Empty;
            }

            var themePath = Path.Combine(_themesDirectory, themeName);
            if (File.Exists(themePath))
            {
                return themePath;
            }

            _logService.Warning($"主题文件不存在: {themePath}");
            return string.Empty;
        }
    }
}
