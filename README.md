# BOOKpandoc 使用指南

官网：[http://bookpandoc.morlvoid.pro/](http://bookpandoc.morlvoid.pro/)
<br>
（抛砖引玉qwq欢迎各路大佬fork或提交PR）

<img src="https://www.blog-image.morlvoid.pro/blog/20260326231514432.png" width="600px" />

## 介绍

BOOKpandoc 是一个用于将 Markdown 文档转换为多种格式电子书的工具，借助于Pandoc的强大功能实现的GUI界面，（理想情况下）支持 PDF、EPUB、DOCX 和 HTML 格式。（现在非理想情况仅支持EPUB和HTML格式）。
由于作者更擅长前端开发，目前v1.0.0版本仅支持 Windows 平台，且.md转HTML格式最为完善，其他格式是作者盲点，需要等待后续版本更新。目前内置2个默认css模板（novel.css和default.css。default.css为Morlvoid制作的黑白小说主题。）

## 主要特性

- 支持多种输出格式：PDF、EPUB、DOCX、HTML
- 简洁直观的用户界面
- 拖拽文件支持
- 自动主题管理
- 封面图片支持（EPUB和HTML）
- 完善的日志系统
- 自动依赖检测和提示

## 安装说明

**重要提示：BOOKpandoc v1.0.0 已采用独立发布模式，无需安装 .NET 运行时，双击即用！**

### 快速开始

1. 下载最新版本的 BOOKpandoc.exe
2. 双击运行程序
3. 点击"下载 Pandoc"按钮安装 Pandoc 工具（首次使用需要）
4. 选择.md格式的文件，选择css文件，设置输出文件位置，点击"导出"按钮生成电子书

### 系统要求

- Windows 10 或更高版本（64位系统）
- 无需安装 .NET 运行时（已内置）
- 需要网络连接下载 Pandoc（首次使用）

### Pandoc 安装

程序支持自动下载 Pandoc，如果自动下载失败，请手动下载并安装：
- 下载地址：https://pandoc.org/installing.html
- 选择 Windows 版本安装包

## 使用指南

### 基本操作

1. **添加章节**：点击"添加文件"按钮或拖拽 Markdown 文件到列表中
2. **调整顺序**：使用"上移"和"下移"按钮调整章节顺序
3. **设置基本信息**：填写书名、作者、选择输出格式和主题
4. **设置输出目录**：点击"浏览"按钮选择输出目录
5. **添加封面**：点击"选择"按钮选择封面图片（支持 EPUB 和 HTML）
6. **生成电子书**：点击"导出"按钮生成电子书

### 高级设置（功能未完善）

- **Word 模板**：选择自定义的 DOCX 模板文件来定义样式


### 封面设置

- **EPUB**：直接选择图片作为封面，程序会自动处理
- **HTML**：选择图片作为封面，程序会在生成的 HTML 开头插入封面图片
- **DOCX**：建议在 Word 模板中设置封面，或手动添加

### 主题使用

- 程序内置了"novel.css和default.css"小说主题
- 对于 HTML，主题会被内嵌到 HTML 文件中

### 阅读器推荐

- **HTML**：使用任何现代网页浏览器

## 常见问题

### Pandoc 未找到
- 点击"下载 Pandoc"按钮安装
- 或手动下载并安装 Pandoc

### 转换失败
- 检查 Markdown 文件格式是否正确
- 检查 Pandoc 是否正确安装
- 检查输出目录是否有写入权限

### 样式不生效
- 确保选择了正确的主题
- 对于 EPUB，使用支持 CSS 的阅读器
- 对于 DOCX，使用自定义模板文件

## 技术支持

如果遇到问题，请查看日志文件（位于程序目录下的 logs 文件夹），或联系开发者:morlvoid@qq.com
欢迎fork和提交Pull Request。也欢迎投稿新的模板主题到邮箱：morlvoid@qq.com

## 开发者指南

### 构建独立发布版本

BOOKpandoc 采用独立发布模式，生成的可执行文件包含完整的 .NET 运行时，用户无需安装任何依赖。

**构建命令：**
```bash
dotnet publish -c Release
```

**构建输出：**
- 输出目录：`bin\Release\net6.0-windows\win-x64\publish\`
- 主要文件：`BOOKpandoc.exe`（约 367MB，包含完整运行时）
- 附加文件：`themes/` 文件夹（包含CSS主题）

**项目配置：**
- 目标运行时：win-x64
- 发布模式：独立发布（Self-Contained）
- 单文件发布：启用
- 包含 .NET 运行时：是

### 项目结构

```
BOOKpandoc/
├── BOOKpandoc/          # 主项目目录
│   ├── Helpers/         # 辅助工具类
│   ├── Models/          # 数据模型
│   ├── Services/         # 核心服务
│   ├── themes/          # CSS主题文件
│   └── BOOKpandoc.csproj # 项目配置
├── README.md            # 项目说明
└── .gitignore          # Git忽略规则
```

### 技术栈

- .NET 6.0 WPF
- Pandoc 文档转换引擎
- GongSolutions.WPF.DragDrop（拖拽支持）
- System.Text.Encoding.CodePages（编码支持）
