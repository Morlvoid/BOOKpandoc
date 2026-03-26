using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace BOOKpandoc;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // 注册编码提供程序，支持 UTF-8 等编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
