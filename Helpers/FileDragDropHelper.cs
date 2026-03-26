using System.Windows;
using System.Windows.Controls;

namespace BOOKpandoc.Helpers
{
    public static class FileDragDropHelper
    {
        public static bool IsValidFileDrop(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0)
            {
                return false;
            }

            // 检查是否有.md文件或.css文件
            return files.Any(file => file.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".css", StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> GetDroppedFiles(DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            return files?.ToList() ?? new List<string>();
        }
    }
}
