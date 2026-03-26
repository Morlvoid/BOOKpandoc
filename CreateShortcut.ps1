# Create shortcut with icon
param(
    [string]$TargetPath = "",
    [string]$ShortcutPath = "",
    [string]$IconPath = "",
    [string]$Description = "BOOKpandoc - E-book Generation Tool"
)

if ([string]::IsNullOrEmpty($TargetPath)) {
    $TargetPath = Join-Path $PSScriptRoot "bin\Release\net6.0-windows\win-x64\publish\BOOKpandoc.exe"
}

if ([string]::IsNullOrEmpty($ShortcutPath)) {
    $ShortcutPath = Join-Path $PSScriptRoot "BOOKpandoc.lnk"
}

if ([string]::IsNullOrEmpty($IconPath)) {
    $IconPath = Join-Path $PSScriptRoot "BOOKpandoc.ico"
}

Write-Host "Creating shortcut..."
Write-Host "Target path: $TargetPath"
Write-Host "Shortcut path: $ShortcutPath"
Write-Host "Icon path: $IconPath"

try {
    $WshShell = New-Object -ComObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut($ShortcutPath)
    $Shortcut.TargetPath = $TargetPath
    $Shortcut.WorkingDirectory = Split-Path $TargetPath
    $Shortcut.Description = $Description
    
    if (Test-Path $IconPath) {
        $Shortcut.IconLocation = $IconPath
        Write-Host "Icon set successfully"
    } else {
        Write-Warning "Icon file not found: $IconPath"
    }
    
    $Shortcut.Save()
    Write-Host "Shortcut created successfully: $ShortcutPath"
} catch {
    Write-Error "Failed to create shortcut: $_"
    exit 1
}
