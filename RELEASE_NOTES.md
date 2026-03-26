# BOOKpandoc v1.0.0 发布说明

## 版本信息
- 版本号：v1.0.0
- 发布日期：2026-03-26
- 平台：Windows 10+ (64位)
- 架构：win-x64

## 主要特性

### ✨ 新功能
- 独立发布模式：无需安装 .NET 运行时，双击即用
- 支持 Markdown 转 EPUB 和 HTML 格式
- 内置两个 CSS 主题：novel.css 和 default.css
- 拖拽文件支持
- 自动主题管理
- 封面图片支持（EPUB和HTML）
- 完善的日志系统
- 自动依赖检测和提示

### 🎨 主题系统
- **novel.css**：标准小说主题，支持响应式设计
- **default.css**：黑白小说主题，由 Morlvoid 制作

### 🔧 技术特性
- 基于 .NET 6.0 WPF 框架
- 使用 Pandoc 文档转换引擎
- 完整的异常处理机制
- 详细的日志记录
- 用户友好的错误提示

## 系统要求

### 最低配置
- 操作系统：Windows 10 或更高版本（64位）
- 内存：4GB RAM（推荐 8GB）
- 硬盘空间：500MB 可用空间
- 网络：首次使用需要下载 Pandoc

### 推荐配置
- 操作系统：Windows 11（64位）
- 内存：8GB RAM 或更多
- 硬盘空间：1GB 可用空间
- 网络：稳定的互联网连接

## 安装说明

### 快速安装
1. 下载 `BOOKpandoc.exe`（约 367MB）
2. 双击运行程序
3. 首次使用时点击"下载 Pandoc"按钮
4. 开始使用！

### 手动安装 Pandoc
如果自动下载失败，请手动安装：
1. 访问 https://pandoc.org/installing.html
2. 下载 Windows 版本安装包
3. 运行安装程序，按照提示完成安装

## 使用指南

### 基本操作
1. **添加章节**：点击"添加文件"按钮或拖拽 Markdown 文件到列表中
2. **调整顺序**：使用"上移"和"下移"按钮调整章节顺序
3. **设置基本信息**：填写书名、作者、选择输出格式和主题
4. **设置输出目录**：点击"浏览"按钮选择输出目录
5. **添加封面**：点击"选择"按钮选择封面图片（支持 EPUB 和 HTML）
6. **生成电子书**：点击"导出"按钮生成电子书

### 支持的格式
- **EPUB**：完全支持，推荐使用
- **HTML**：完全支持，样式最完善
- **PDF**：实验性支持，需要额外配置
- **DOCX**：实验性支持，需要额外配置

## 已知问题

### 当前限制
- 仅支持 Windows 平台
- PDF 和 DOCX 格式支持有限
- 需要网络连接下载 Pandoc（首次使用）

### 计划改进
- 跨平台支持（macOS、Linux）
- 完善的 PDF 和 DOCX 支持
- 离线 Pandoc 包
- 更多主题和样式选项
- 批量处理功能

## 文件说明

### 主要文件
- `BOOKpandoc.exe`：主程序（包含完整运行时）
- `themes/`：CSS 主题文件夹
  - `novel.css`：标准小说主题
  - `default.css`：黑白小说主题

### 输出文件
- `logs/`：日志文件目录
- `settings.json`：用户设置文件

## 技术支持

### 获取帮助
- 邮箱：morlvoid@qq.com
- GitHub Issues：[项目地址]
- 查看日志文件：程序目录下的 `logs` 文件夹

### 贡献指南
欢迎提交 Pull Request 和 Issue！
- 主题投稿：morlvoid@qq.com
- 功能建议：通过 GitHub Issues
- Bug 报告：提供详细日志和复现步骤

## 开发者信息

### 构建信息
- 构建工具：.NET 6.0 SDK
- 构建命令：`dotnet publish -c Release`
- 输出目录：`bin\Release\net6.0-windows\win-x64\publish\`
- 发布模式：独立发布（Self-Contained）

### 项目依赖
- GongSolutions.WPF.DragDrop (2.2.0)
- System.Text.Encoding.CodePages (6.0.0)

## 许可证

本项目采用 [许可证类型] 许可证。

## 更新日志

### v1.0.0 (2026-03-26)
- 🎉 首次发布
- ✅ 支持 Markdown 转 EPUB 和 HTML
- ✅ 内置两个 CSS 主题
- ✅ 独立发布模式，无需安装 .NET
- ✅ 完善的异常处理和日志系统
- ✅ 拖拽文件支持
- ✅ 封面图片支持

---

感谢使用 BOOKpandoc！如有任何问题或建议，欢迎联系。
