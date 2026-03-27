[Setup]
AppName=BOOKpandoc
AppVersion=1.0.1
AppPublisher=Morlvoid
AppPublisherURL=https://github.com/Morlvoid/BOOKpandoc
AppSupportURL=https://github.com/Morlvoid/BOOKpandoc/issues
AppUpdatesURL=https://github.com/Morlvoid/BOOKpandoc/releases
DefaultDirName={pf}\BOOKpandoc
DefaultGroupName=BOOKpandoc
AllowNoIcons=yes
LicenseFile=README.md
OutputBaseFilename=BOOKpandoc_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
UninstallDisplayIcon={app}\BOOKpandoc.exe
SetupIconFile=BOOKpandoc.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1

[Files]
Source: "bin\Release\net6.0-windows\win-x64\publish\BOOKpandoc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\net6.0-windows\win-x64\publish\themes\*"; DestDir: "{app}\themes"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\BOOKpandoc"; Filename: "{app}\BOOKpandoc.exe"; IconFilename: "{app}\BOOKpandoc.exe"
Name: "{group}\{cm:UninstallProgram,BOOKpandoc}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\BOOKpandoc"; Filename: "{app}\BOOKpandoc.exe"; Tasks: desktopicon; IconFilename: "{app}\BOOKpandoc.exe"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\BOOKpandoc"; Filename: "{app}\BOOKpandoc.exe"; Tasks: quicklaunchicon; IconFilename: "{app}\BOOKpandoc.exe"

[Run]
Filename: "{app}\BOOKpandoc.exe"; Description: "{cm:LaunchProgram,BOOKpandoc}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\logs"