@echo off
echo ========================================
echo BOOKpandoc v1.0.0 发布包生成脚本
echo ========================================
echo.

echo [1/4] 清理旧的构建文件...
dotnet clean
if %errorlevel% neq 0 (
    echo 清理失败！
    pause
    exit /b 1
)
echo 清理完成。
echo.

echo [2/4] 恢复依赖包...
dotnet restore
if %errorlevel% neq 0 (
    echo 依赖恢复失败！
    pause
    exit /b 1
)
echo 依赖恢复完成。
echo.

echo [3/4] 构建独立发布版本...
dotnet publish -c Release
if %errorlevel% neq 0 (
    echo 构建失败！
    pause
    exit /b 1
)
echo 构建完成。
echo.

echo [4/4] 验证发布文件...
if exist "bin\Release\net6.0-windows\win-x64\publish\BOOKpandoc.exe" (
    echo 发布文件验证成功！
    echo.
    echo ========================================
    echo 发布包位置：
    echo bin\Release\net6.0-windows\win-x64\publish\
    echo ========================================
    echo.
    echo 主要文件：
    dir "bin\Release\net6.0-windows\win-x64\publish\BOOKpandoc.exe" | findstr "BOOKpandoc.exe"
    echo.
    echo 发布完成！
) else (
    echo 发布文件验证失败！
    pause
    exit /b 1
)

pause
