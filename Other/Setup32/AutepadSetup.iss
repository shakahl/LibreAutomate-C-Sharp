; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Autepad C#"
#define MyAppNameShort "Autepad"
#define MyAppVersion "0.3.1 (2021-12-25)"
#define MyAppPublisher "Gintaras Did�galvis"
#define MyAppURL "https://www.quickmacros.com/au/help/"
#define MyAppExeName "Au.Editor.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{091E96D4-5062-4119-9F27-76D4BD0AAF79}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppNameShort}
DefaultGroupName={#MyAppName}
OutputDir=..\..\_
OutputBaseFilename=AutepadSetup
Compression=lzma/normal
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
;ArchitecturesAllowed=x64 x86
MinVersion=0,6.1
DisableProgramGroupPage=yes
AppMutex=Au.Editor.Mutex.m3gVxcTJN02pDrHiQ00aSQ
UsePreviousGroup=False
DisableDirPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Q:\app\Au\_\Default\*"; DestDir: "{app}\Default"; Excludes: ".*"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Q:\app\Au\_\Templates\files\*"; DestDir: "{app}\Templates\files"; Flags: ignoreversion recursesubdirs
Source: "Q:\app\Au\_\Templates\files.xml"; DestDir: "{app}\Templates"; Flags: ignoreversion
Source: "Q:\app\Au\_\Cookbook\files\*"; DestDir: "{app}\Cookbook\files"; Flags: ignoreversion recursesubdirs
Source: "Q:\app\Au\_\Cookbook\files.xml"; DestDir: "{app}\Cookbook"; Flags: ignoreversion

Source: "Q:\app\Au\_\Au.Editor.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Editor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Task.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Controls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Controls.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Net45.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Au.Net45.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\Setup32.dll"; DestDir: "{app}"; Flags: ignoreversion

Source: "Q:\app\Au\_\Roslyn\*.dll"; DestDir: "{app}\Roslyn"; Flags: ignoreversion
Source: "Q:\app\Au\_\64\Au.AppHost.exe"; DestDir: "{app}\64"; Flags: ignoreversion
Source: "Q:\app\Au\_\64\AuCpp.dll"; DestDir: "{app}\64"; Flags: ignoreversion
Source: "Q:\app\Au\_\64\Scintilla.dll"; DestDir: "{app}\64"; Flags: ignoreversion
Source: "Q:\app\Au\_\64\Lexilla.dll"; DestDir: "{app}\64"; Flags: ignoreversion
Source: "Q:\app\Au\_\64\sqlite3.dll"; DestDir: "{app}\64"; Flags: ignoreversion
Source: "Q:\app\Au\_\32\Au.AppHost.exe"; DestDir: "{app}\32"; Flags: ignoreversion
Source: "Q:\app\Au\_\32\AuCpp.dll"; DestDir: "{app}\32"; Flags: ignoreversion
Source: "Q:\app\Au\_\32\sqlite3.dll"; DestDir: "{app}\32"; Flags: ignoreversion
Source: "Q:\app\Au\_\System.ServiceProcess.ServiceController.*"; DestDir: "{app}";
Source: "Q:\app\Au\_\System.Management.*"; DestDir: "{app}";

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

Source: "Q:\app\Au\_\default.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\Other\Data\doc.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\Other\Data\ref.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\Other\Data\winapi.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\icons.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "Q:\app\Au\_\xrefmap.yml"; DestDir: "{app}"; Flags: ignoreversion

[InstallDelete]
;Type: filesandordirs; Name: "{app}\Default"
Type: files; Name: "{app}\Au.CL.exe"
Type: files; Name: "{app}\Au.Task32.exe"
;Roslyn dlls moved to subfolder Roslyn
Type: files; Name: "{app}\Microsoft.*.dll"
Type: files; Name: "{app}\System.*.dll"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
;Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
;register app path
Root: HKLM; Subkey: Software\Microsoft\Windows\CurrentVersion\App Paths\Au.Editor.exe; ValueType: string; ValueData: {app}\Au.Editor.exe; Flags: uninsdeletevalue uninsdeletekeyifempty

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure Cpp_Install(step: Integer; dir: String);
external 'Cpp_Install@files:Setup32.dll cdecl setuponly delayload';

procedure Cpp_Uninstall(step: Integer);
external 'Cpp_Uninstall@{app}\Setup32.dll cdecl uninstallonly delayload';

function InitializeSetup(): Boolean;
begin
  Cpp_Install(0, '');
  Result:=true;
end;

function InitializeUninstall(): Boolean;
begin
  Cpp_Uninstall(0);
  Result:=true;
end;

procedure CurStepChanged(CurStep: TSetupStep);
//var
//  s1: String;
begin
//  s1:=Format('%d', [CurStep]);
//  MsgBox(s1, mbInformation, MB_OK);
  
  case CurStep of
    ssInstall:
    begin
      //Cpp_Install(1, ExpandConstant('{app}\'));
    end;
    ssPostInstall:
    begin
      Cpp_Install(2, ExpandConstant('{app}\'));
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  s1: String;
begin
//  s1:=Format('%d', [CurUninstallStep]);
//  MsgBox(s1, mbInformation, MB_OK);
  
  case CurUninstallStep of
    usUninstall:
    begin
      Cpp_Uninstall(1);
      UnloadDLL(ExpandConstant('{app}\Setup32.dll'));
    end;
    usPostUninstall:
    begin
      s1:=ExpandConstant('{app}');
      if DirExists(s1) and not RemoveDir(s1) then begin RestartReplace(s1, ''); end;
    end;
  end;
end;
