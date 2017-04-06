These are reviewed in QM and other projects. Also review in System.

str::GetWinClass, GetClassName
* GetWindowText, WM_GETTEXT
FromWinAnyThread
FromWin
WinTest, WinTestMulti
SetWindowText
SetDlgItemText
GetDlgItemText
WM_SETTEXT
SetWinText
SuperClass, Subclass
EM_REPLACESEL, EM_...
dlg::GetText, SetText, etc
CStr::Escape, str.escape
LB_ADDSTRING, CB_ADDSTRING, CB_INSERTSTRING, CBEM_INSERTITEM, etc
Treeview, listview, etc
ListView_, etc
CBstr
WideCharToMultiByte, MultiByteToWideChar
mes, inp, inpp, list.
Dialog definition.
outp, str.setsel
CreateRegExpMenu, RegExpMenu, GetLastSelectedMenuItem
AppendMenu, InsertMenuItem, PopupMenu, GetMenuString
MenuGetString, MenuSetString
SpecFoldersMenu
Window messages with text parameters
QM window as Unicode
CreateWindowEx, CreateControl
TBN_
tooltip text
menu bar triggers
WM_NOTIFY, OnNotify, wm_notify etc
reviewed all dialogs
DispatchMessages
CRegArr
SHGetPathFromIDList, FromPIDL
dialog.cpp
* [160] in CreateRegExpMenu etc
* GetUserName, GetComputerName
registry functions
getwinexe, Processes
* SHChangeNotify
dir, searchpath
mkdir (CreateDirectory)
create/get shortcut, including Msi functions (get Office shortcut target)
* CreateFile
dospath, GetLongPathName, GetShortPathName
command line, Properties-> Shortcut, Command line, Scheduler
OpenFileDialog, SHBrowseForFolder, SpecFolderMenu, etc
exename functions
file triggers
process triggers
ucase, lcase (Unicode characters)
FormatMessage (CStr::FromError, event log triggers)
tsm
PIDL functions
escape URL: maybe convert to ANSI. Don't know what characters can be used in URL.
Unicode in menus and toolbars.

* in System OK too.

------------------------------------------------

These are partially reviewed


------------------------------------------------

These are not reviewed



------------------------------------------------

TODO


----------------

Unicode not used with:

LOGFONT, GetObject
Type library identifiers. (TODO: test identifiers containing non ASCII characters).
service functions
RegisterClipboardFormat
Shell_NotifyIcon (we only display "Quick Macros")
pdh functions
desktop, workstation, security, etc functions
setup and qmsetup.dll. Assume will not try to install to an Unicode folder.
qmtul. Would need quite much work.

----------------

Inconsistencies

