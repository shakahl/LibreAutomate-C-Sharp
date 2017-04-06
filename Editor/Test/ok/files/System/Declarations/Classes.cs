 /?

 Simple Windows handle classes
class __Handle handle ;;A kernel HANDLE (event, mutex, file, process, etc). Calls CloseHandle in dtor.
class __HFile handle ;;File handle. Creates or opens a file using API CreateFile. Calls CloseHandle in dtor.
class __GlobalMem handle ;;QM 2.2.1. HGLOBAL handle. Calls GlobalFree in dtor.
class __GdiHandle handle ;;QM 2.2.1. HGDIOBJ handle (bitmap, pen, brush, font, etc). Calls DeleteObject in dtor.
class __Hicon handle ;;QM 2.3.0. HICON handle. Calls DestroyIcon in dtor. Can be used with GetFileIcon, GetWindowIcon etc.
class __ImageList handle ;;QM 2.3.0. HIMAGELIST handle. Loads imagelist created with QM imagelist editor. Calls ImageList_Destroy in dtor.
class __Hdc dc ;;QM 2.3.4. HDC handle. Gets window or screen device context. Calls ReleaseDC in dtor. For memory DC instead use __MemBmp.
class __Accelerators hacc ;;QM 2.3.5. HACCEL handle. Creates accelerator table. Calls DestroyAcceleratorTable in dtor.
class __WindowsHook hhook ;;QM 2.4.2. HHOOK handle (SetWindowsHookEx). Calls UnhookWindowsHookEx in dtor.
class __HProcess :__Handle ;;QM 2.4.3. Process handle. Gets process handle using API OpenProcess. Calls CloseHandle in dtor.

 Mics
class __Font :__GdiHandle'__ ;;QM 2.2.1. Font for dialog controls and not only. Also can be used as HFONT handle.
class __MemBmp bm dc -oldbm ;;QM 2.2.1. Memory device context with bitmap, to use with GDI drawing API. See also __Hdc.
class __ProcessMemory -!*m_mem -m_hproc ;;QM 2.3.1. Allocates, writes and reads memory in other process.
class __Tcc -#*m_code -m_flags -str'm_ew -m_iid f ;;QM 2.3.2. Compiles C code.
class __RegisterHotKey -m_hwnd -m_hkid ;;QM 2.3.2. Registers a hotkey using API RegisterHotKey. Calls UnregisterHotKey in dtor.
class __Stream IStream'is ;;QM 2.3.2. Makes easier to use IStream interface.
class __MapIntStr ARRAY(LPSTRINT)a -str'm_s ;;QM 2.3.2. A simple integer-string map. Not optimized for large item count.
class __Settings -IStringMap'm_m -ICsv'm_c -~m_rName -~m_rKey ;;QM 2.3.3. Stores macro settings in registry; displays in grid control.
class __Tooltip htt __hwnd ARRAY(STRINT)__a -m_flags -m_hwndPrev ;;QM 2.3.4. Creates tooltips.
class __RegisterWindowClass atom baseClassWndProc baseClassCbWndExtra -m_hicon -m_hiconSm ;;QM 2.3.4. Registers window class. Calls UnregisterClass in dtor.
class __OnScreenRect -m_hwnd -RECT'm_pr __brush __style ;;QM 2.3.4. Draws on-screen rectangle.
class __MinimizeDialog -ARRAY(int)m_a ;;QM 2.3.4. Minimizes/restores dialog. Can be used eg when drag/capturing an object from screen.
class __ComActCtx -m_hctx ;;QM 2.3.5. A COM activation context. Used by __ComActivator.
class __ComActivator -m_cookie ;;QM 2.3.5. Allows to use a COM component without registration.
class __SharedMemory !*mem -m_hmapfile ;;QM 2.3.5. Memory that can be used by multiple processes.
class __TempFile ~m_file ;;QM 2.3.5. Auto deletes file in dtor.
class __Drag -m_hwnd -m_mouseButton -!m_started !dropped MSG'm POINT'p mk cursor ;;QM 2.4.1. Manages non-OLE drag-drop.

 Files
#opt hidedecl 1
interface# IEnumFiles :IUnknown
	Begin($path [flags]) ;;flags: 0 files, 1 folders, 2 any, 4 +subfolders, 0x100 skip symbolic-link subfolders, 0x200 return full path
	End ;;optional
	$Next ;;the main function. Finds first or next matching file. Returns filename, or 0 no more.
	SkipChildren
	[g]#Level ;;0, 1...
	[g]WIN32_FIND_DATAU*Data
	[g]$FullPath
	[g]$RelativePath
	{3426CF3C-F7C1-4322-A292-463DB8729B58}
dll "qm.exe" IEnumFiles'CreateEnumFiles
#opt hidedecl 0
class Dir IEnumFiles'__e WIN32_FIND_DATAU*fd : FE_Dir ;;Finds/enumerates files and gets properties. To insert code, use dialog 'Enumerate files'.

class Sqlite -!*m_db : SqliteStatement "?str format SqlEscape" ;;QM 2.3.2. SQLite database functions. 
#opt hidedecl 1
class SqliteStatement !*p -!*conn ;;QM 2.3.2. Makes easier to use SQLite prepared statements.
#opt hidedecl 0
class File m_file ;;Wraps MSVCRT file IO functions such as fopen, fread, fwrite. For Windows file IO API instead use __HFile class. 

 Internet
class __HInternet handle ;;A Windows HINTERNET handle. Automatically calls InternetCloseHandle.
class __Wininet -m_hitop -m_hi -!m_isFtp -m_dlg -m_hdlg -~m_dlgTitle -m_fa -m_fparam ~lasterror ;;Base class for Http and Ftp.
type __FTP_FIND_DATA hfind flags WIN32_FIND_DATA'fd
class Ftp :__Wininet'__ -__FTP_FIND_DATA*m_fd
type POSTFIELD str'name str'value !isfile
class Http :__Wininet'__ -ARRAY(POSTFIELD)m_ap ;;Internet download/upload.
type __WininetT hdlg hthread __Wininet*wi []Http*h []Ftp*f func *a r
type __INTSETTINGS !useproxy str'proxy_name str'proxy_bypass str'user_agent
__INTSETTINGS+ __intsett

 System, window
class Dde -m_idinst -m_conv
#opt hidedecl 1
class RegKey hkey ;;A registry key handle (HKEY). Makes easier to open key. Automatically calls RegCloseKey.
type FFNODEINFO BSTR'bName BSTR'bValue numChildren _EventId @nodeType @_NamespaceId
class FFNode FFDOM.ISimpleDOMNode'node
#opt hidedecl 0
type __TRAYIC ~icfile __Hicon'hicon
class Tray -NOTIFYICONDATAW'nd -m_flags -m_hwnd __m_func ~__m_onclick ~__m_onrclick param -ARRAY(__TRAYIC)m_a ;;Adds tray icon.
class MenuPopup -m_h ;;QM 2.3.2
class DateTime long't : ">DateTime_static" ;;QM 2.3.2

 Dialog controls
#opt hidedecl 1
class DlgControl -h ;;QM 2.3.2. Base of other dialog control classes.
class DlgGrid :DlgControl'__ ;;QM 2.3.2. Manages a QM_Grid control.
#opt hidedecl 0

 WindowText
def WTI_INVISIBLE 1
def WTI_MULTILINE 2
type WTI hwnd $txt txtLen RECT'rt RECT'rv @fontHeight !flags !api apiFlags color bkColor
def WT_SPLITMULTILINE 1
def WT_JOIN 2
def WT_JOINMORE 4
def WT_NOCHILDREN 8
def WT_VISIBLE 16
def WT_REDRAW 32
def WT_SORT 64
def WT_SINGLE_COORD_SYSTEM 128
def WT_WAIT_BEGIN 0x100
def WT_WAIT 0x300
def WT_WAIT_END 0x200
def WT_NOCLIPTEXT 0x400
def WT_GETBKCOLOR 0x800
#opt hidedecl 1
interface# ITextCapture :IUnknown
	Begin(hwnd)
	#Capture(WTI**arr [hwnd] [flags] [RECT*r])
	End()
	[p]Codepage(codepage)
dll- "$qm$\qmtc32.dll" ITextCapture'CreateTextCapture
#opt hidedecl 0
class WindowText ;;QM 2.3.4. Captures window text.
	WTI*a n
	-ITextCapture'm_tc
	-m_hwnd -m_flags
	-RECT'm_r -RECT*m_rp
	-m_cbFunc -m_cbParam
	-!m_captured -!m_dpiScaled

 CsScript
#opt hidedecl 1
interface# ICsScript :IDispatch
	Dispose
	SetOptions(setGlobal BSTR'optionsList)
	AddCode(BSTR'code [flags] [BSTR'sourceFile])
	Compile(BSTR'code BSTR'assemblyFile [flags] [BSTR'sourceFile])
	Load(BSTR'assemblyFile)
	LoadFromMemory(SAFEARRAY*data)
	`Call(BSTR'name SAFEARRAY*parameters)
	IDispatch'CreateObject(BSTR'className)
dll "qm.exe" #CreateCsScript ICsScript*x *step
STRINT+ __cs_sett
#opt hidedecl 0
class CsScript ICsScript'x ;;QM 2.3.5. Compiles and executes C# or VB.NET code.

 Obsolete
#opt obsoletedecl 1
type GdiObject :__GdiHandle ;;Alias for __GdiHandle.
#opt obsoletedecl 0

 Declare on demand
ref __classes2 "Classes2" 10

 PRIVATE

type __DIALOGHANDLE handle flags
type __DIALOGCOLORS
	func
	bkFlags ;;0 none, 1 horz grid, 2 vert grid, 3 one color or image
	color color2 __GdiHandle'hBrush __GdiHandle'hBrushForControl
	ARRAY(POINT)'a
type __DIALOG
	dlgproc str*controls hwndowner flags param x y
	__Hicon'hicon16 __Hicon'hicon32 hmenu haccel haccel2
	ARRAY(int)acid
	ARRAY(__DIALOGHANDLE)ah
	flags2 ;;1 DT_Init called, 2 initialized, 4 DT_GetControls called, 8 DT_Ok/DT_Cancel called, 16 DT_Init called explicitly (old version of user dlgproc), 0x100 hidden or inactive, 0x200 don't disable owner
	style
	__DIALOGCOLORS*colors
	__Tooltip*tt ttFlags ttTime
type __DIALOGPARAM hDlg message wParam lParam
word+ __atom_dialogdata

type __FAVRET dlg ctrl ~icon
type __DispinfoText -ARRAY(BSTR)m_a -m_i
class __strt :str's
type __HWNDFLAGS hwnd flags
class __IdStringParser ARRAY(__HWNDFLAGS)a ~warnings
type __MSGLOOP flags cbFunc cbParam __Accelerators'accel accelHwnd
type __ACTCTXMAP __ComActCtx'ac ~manifest

type __SubDD offset len innerOffset innerLen optionsOffset
type __Sub subOffset subLen codeOffset codeLen ~name __SubDD'dd ;;__Subs.a array elements
type __SubsEVENTS iDP wmBegin wmEnd cmdBegin cmdEnd
class __Subs ~sText ARRAY(__Sub)a __SubsEVENTS'e ;;Finds sub-functions in macro text. Optionally finds dialog definitions etc. e finds dialog procedure for events.
