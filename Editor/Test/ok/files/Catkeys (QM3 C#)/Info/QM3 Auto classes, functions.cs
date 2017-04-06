 NAMESPACES

namespace Catkeys.
 Classes used in the library/program and scripts. Eg String_, Output.

namespace Catkeys.Automation.
 High-level macro/automation classes, used mostly only in scripts, seldom in library/program.

namespace Catkeys.Util.
 Programming util functions, used mostly only in library/program, seldom in scripts.

 In program/library files:
using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Auto = Catkeys.Automation;

 In scripts:
using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Automation; using Auto = Catkeys.Automation;
using static Catkeys.Automation.NoClass;


 CLASSES OF Catkeys

 Static classes

NoClass
 Contains copies of often used static functions of other classes.
 The script and library templates contain 'using static Catkeys.NoClass;'.
 Static methods:
	Out (Output.Write), Meow (Show.MessageDialog),
	RunScript,

Output
 Name not used in .NET.
 Static methods:
	Write, WriteFunc, WriteDirectly, Clear, RedirectConsoleOutput, RedirectDebugOutput, WriteStatusBar
 Static properties:
	Writer, AlwaysOutput, IsConsoleProcess

Show
 Name not used in .NET.
 Static methods:
	MessageDialog, InputDialog, TextDialog, ListDialog, PasswordDialog, UserPasswordDialog,
	OnScreen, HideOnScreen, Tip, HideTip, OnScreenRect,
	FileOpenDialog, FileSaveDialog, FolderDialog, IconDialog, ColorDialog, FontDialog,
	DropdownList, Help,
 Static properties:
	DefaultMonitor
 Classes:
	TaskDialog (use API TaskDialog[Indirect])
	 Static methods:
		Simple, ButtonList, OptionList
	 Instance methods:
		Show
	 And many instance properties.

String_
 Contains String extension methods.
 Static methods:

App
 Name not used in .NET. Name Application used in .NET forms etc. Name Program used by VS in new cs files.
 Static methods:
	RunScript, RunText,
	RunScriptAt, RunScriptAfter, RunScriptTimer, RunThreadTimer,
	SetPrivilege, CompileFolderItems,
 Static properties:
	Scripts, Tasks,
 Static classes:
	Remote (QM2 net): RunScript, RunServer, SendScript
	Editor: Exit, Restart, Show, Window (prop), CodeControl (prop), InsertCode,
	CatkeysFile: Reload, Import, Export, Sqlite (prop), Path (prop)
	Triggers: Enable, Restore
 Instance classes:
	CatkeysScripts: [name], Add
	CatkeysScript: Run, Enable, Delete, Copy, Move, Rename, FolderItems (prop),
	CatkeysTasks: [name], EndTask, EndAll,
	CatkeysTask: End, Pause, Resume, Name (prop)
 Constants/readonly
	Version, VersionString, IsExe, IsPortable, Path, Directory, IsAdmin, IsUiAccess

ThisTask
	AddTrayIcon, End, Pause, CatkeysScript (prop)
	DebugBreak (deb)

Calc
 Name not used in .NET.
	HashCrc32, HashMD5,
	BlowfishEncrypt, BlowfishDecrypt, Base64Encode, Base64Decode, HexEncode, HexDecode, LzoCompress, LzoDecompress,
	RandomInt, RandomDouble, RandomString, Guid,
	MakeLparam, MulDiv, Percent,

Time
 Name not used in .NET.
	Milliseconds (prop) (GetTickCount or timeGetTime), Microseconds (prop),
	First, Next, Write, NextWrite ;;instead of Perf.First etc

 Instance classes

Csv (try Microsoft.VisualBasic.FileIO.TextFieldParser)
Sqlite
SqliteStatement


 CLASSES OF Catkeys.Automation

 Static classes

NoClass
 Contains copies of often used static functions of other classes.
 The script template contains 'using static Catkeys.Automation.NoClass;'.
 Static methods:
	Keys, Text, Paste,
	Click, DoubleClick, RightClick,
	Run (Shell.Run),
	Wait (WaitFor.Wait),

Input
 Name not used in .NET. Name Keyboard used: class System.Windows.Input.Mouse (WPF), enum Microsoft.DirectX.DirectInput.Mouse, a VB class.
 Static methods:
	Keys, Text, CopyText, Paste, KeyDown, KeyUp, KeyToggle, IsKey,
	GetTextCursorXY, BlockUserInput, EnterPassword, KeysToArray, KeyNameFromVK, KeyNameToVK,
	GetIdleTime, WaitForIdle,
 Maybe also:
	RemapKeys, DisableCapsLock
 Static properties:
	ModifierKeys (ex: if(Input.ModifierKeys==Input.Ctrl) ...)
 Enums:
	TextSelection (with CopyText and Paste; {None, All, Line})
 Investigated:
   About terms "cursor", "pointer" and "caret":
     Term "cursor" is used for both mouse pointer and text caret.
     Term "caret" used only in Windows programming and is unknown to others. In Control Panel -> Keyboard it is "cursor".
     Terms "mouse cursor" and "mouse pointer" are equally used (or just "pointer"). In Control Panel -> Mouse it is "pointer".
     Decision: in function names and documentation use terms "mouse cursor" and "text cursor" (MouseCursor, TextCursor).

Mouse
 Name used: class System.Windows.Input.Mouse (WPF), enum Microsoft.DirectX.DirectInput.Mouse, a VB class.
 Static methods:
	Move, Click, DoubleClick, RightClick, MiddleClick, X1Click, X2Click, Wheel,
	Drag, RestoreXY, IsButton, GetX, GetY, GetXY, GetCursorId

Files
 Name not used in .NET.
 Static methods:
	RunConsole, CopyFile, MoveFile, RenameFile, DeleteFile, Zip,
 Sub-classes:
	FileInfo

Shell
 Name not used in .NET.
 Static methods:
	Run, CopyFile, MoveFile, RenameFile, DeleteFile, CreateFolder,
	PidlFromString, PidlToString,
	CreateShortcut

Processes
 Name not used in .NET. Name Process used by System.Diagnostics.Process.
 Static methods:
	End, GetList, GetCpuUsage, NameToId, NameFromId, GetIntegrityLevel, GetUacInfo

ScreenImage
 Name Screen used in .NET forms. Name Image used too.
 Static methods:
	Find, GetPixel, Capture, CaptureUI,

Sound
 Name not used in .NET.
 Static methods:
	Wave, Beep, Play, StopPlay, Speak, StopSpeak, Meow (standard msgbox sounds)
 For Beep try Console.Beep(), or don't need.

Computer
 Name used: Microsoft.VisualBasic.Devices.Computer.
 Static methods:
	Shutdown, Restart, LogOff, Sleep, Hibernate, Lock,
	GetCpuUsage, GetDiskUsage, SetDefaultPrinter, SetTime
 Static properties:
	Name, UserName, IsLoggedOn

SystemInfo (join with Computer)
 Name SystemInformation used by .NET forms.

Internet
 Contains just easy-to-use static methods. Users can use System.Net classes.
 Static methods:
	OpenPage (QM2 web), CheckConnection, Download, SendEmail, ReceiveMail, Settings

LogFile
 Name not used in .NET.
 Static methods:
	Write, Delete
 Static properties:
	ScriptDefaultFile, DefaultFile, SizeLimit

Form_
 Static methods:
	SetAutoSizeControls

 See also:

__programming_misc
__qm_dll


 Instance classes

Wnd (maybe better in Catkeys ns)
 See macro "QM3 win and other window functions"
 Static methods:
	Find
 Instance methods:
	VirtualInput.Keys/Text/CopyText/Paste/KeyDown/KeyUp,
	VirtualMouse.Move/Click/DoubleClick/RightClick/MiddleClick/X1Click/X2Click/Wheel,
	MenuCommand,
 Don't add button/combo/etc functions. Instead use UIElem class.
 Maybe add sub-classes for UI objects: Elem, HtmlElem, Button, Combo, Edit. Or use a main container class UI.

UIElem

HtmlElem

ExcelSheet

HtmlDoc (try System.Windows.Forms.HtmlDocument)

TrayIcon

Pcre

CatMenu
 Name Menu used in .NET forms, WPF.
 Or could be Menu. In C# it will be used, not the forms Menu which is obsolete. In VB use Auto.Menu (in C# too, for clarity).

CatBar
 Name ToolBar used in .NET forms, WPF.
 Or could be Toolbar. In C# it will be used, not the forms ToolBar which is obsolete. In VB use Auto.Toolbar (in C# too, for clarity).
 Word 'toolbar' exists and is commonly used. Word 'tool bar' not used.

AutoText
 Name not used in .NET.
 There is no such English/technical word 'autotext'. Instead used either 'AutoText' (eg in Word) or 'auto-text' (eg in Wiki).




 .NET CLASSES THAT REPLACE QM2 CLASSES/FUNCTIONS/TYPELIBS

rset, rget, FE_RegKey, RegGetValues, RegGetSubkeys, RegKey: classes System.Win32.Registry and RegistryKey
GUID, str.Guid, str.FromGUID: class System.Guid
Services: namespace System.ServiceProcess
DateTime: struct DateTime
Database: System.Data namespace
GetCurDir, SetCurDir: Environment.CurrentDirectory
GetUserInfo: Environment.UserName, Environment.MachineName
GetEnvVar, SetEnvVar: Environment.GetEnvironmentVariable, Environment.SetEnvironmentVariable
_command: Environment.GetCommandLineArgs
_error: catch(ExceptionX e)
_hresult: ExceptionX.HResult
_hinst: Marshal.GetHINSTANCE(typeof(ThisClass).Module), Api.GetModuleHandle(null)
_win64: Environment.Is64BitOperatingSystem
_winver, _winnt: Environment.OperatingSystem


 COM COMPONENTS

WSH - add reference to COM component "Windows Script Host..." and use namespace IWshRuntimeLibrary.


 DOWNLOAD .NET COMPONENTS

Json.NET


 REMOVED

Dde class
WindowText class (maybe will be added later if somebody will ask)
Acc class (now use UI Automation, class UIElem; maybe add Acc later)
IniFile class (add only if somebody will ask; see http://stackoverflow.com/questions/217902/reading-writing-an-ini-file)
