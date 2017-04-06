 /
function# hwnd [flags] ;;flags: 1 large (32x32), 8 create empty if fails

 Gets icon that is displayed in window title bar and in its taskbar button.
 Returns icon handle if successful, else 0. Later call DestroyIcon, or assign to a __Hicon variable.

 hwnd - window handle.
 flags (QM 2.4.3) - see above.

 REMARKS
 QM 2.4.3: Supports Windows store app windows (Windows 8/10).

 See also: <GetFileIcon>


int R big=flags&1

 support Windows store apps
if _winver>=0x602
	sel WinTest(hwnd "Windows.UI.Core.CoreWindow[]ApplicationFrameWindow")
		case 1 _i=hwnd
		case 2 if(_winver>=0xA00) _i=sub_sys.GetWindowsStoreAppFrameChild(hwnd)
	if _i and sub_sys.GetWindowsStoreAppId(_i _s 1)
		R=GetFileIcon(_s 0 big)
		if(R) ret R

SendMessageTimeoutW(hwnd WM_GETICON iif(big ICON_BIG ICON_SMALL) 0 SMTO_ABORTIFHUNG 1000 &R)
if(!R) SendMessageTimeoutW(hwnd WM_GETICON iif(big ICON_SMALL ICON_BIG) 0 SMTO_ABORTIFHUNG 1000 &R)
if(!R) R=GetClassLong(hwnd iif(big GCL_HICON GCL_HICONSM))
if(!R) R=GetClassLong(hwnd iif(big GCL_HICONSM GCL_HICON))

if(R) ret CopyIcon(R)
if(flags&8) ret GetFileIcon("" 0 flags&9)
