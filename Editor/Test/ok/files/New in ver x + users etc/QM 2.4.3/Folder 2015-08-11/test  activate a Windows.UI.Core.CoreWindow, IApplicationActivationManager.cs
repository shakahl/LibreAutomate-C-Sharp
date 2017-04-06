 Unsuccessfully tried to activate a Windows.UI.Core.CoreWindow window that does not have an ApplicationFrameWindow.

int w=win("Search" "Windows.UI.Core.CoreWindow")
 act w ;;does not work
outw2 win
SetForegroundWindow(w)
outw2 win ;;0
ret

str sid
if(!sub.GetWindowsStoreAppId(w sid)) ret
 out sid ;;Microsoft.Windows.Cortana_cw5n1h2txyewy!CortanaUI

interface# IApplicationActivationManager :IUnknown
	#ActivateApplication(@*appUserModelId @*arguments options) ;;returns pid
	{2e941141-7f97-4756-ba1d-9decde894a3d}
    HRESULT ActivateForFile(
        [in] LPCWSTR appUserModelId,
        [in] IShellItemArray *itemArray,
        [in, unique] LPCWSTR verb,
        [out] DWORD *processId);
    HRESULT ActivateForProtocol(
        [in] LPCWSTR appUserModelId,
        [in] IShellItemArray *itemArray,
        [out] DWORD *processId);

IApplicationActivationManager man._create("{45BA127D-10A8-46EA-8AB7-56EA9078943C}")
man.ActivateApplication(@sid L"" 2) ;;2 AO_NOERRORUI, 4 AO_NOSPLASHSCREEN ;;does nothing (probably it is just to run, not to activate window). With flag 4 Access is denied.


#sub GetWindowsStoreAppId
function! hwnd str&appID [flags] ;;flags: 1 prepend "shell:AppsFolder\" (to run or get icon), 2 get exe full path if hwnd is not a store app

 Gets Windows store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
 Returns 1 if gets user model id, 2 if gets path, 0 if fails.

if flags&2
	int isApp
	if _winver>=602
		sel WinTest(hwnd "Windows.UI.Core.CoreWindow[]ApplicationFrameWindow")
			case 1 isApp=1
			 case 2 if(_winver>=0xA00) _i=sub.GetWindowsStoreAppFrameChild(hwnd); if(_i) hwnd=_i; isApp=1
	if(!isApp) appID.getwinexe(hwnd 1); ret 2
else if(_winver<0x602) ret

__HProcess hp; if(!hp.Open(hwnd PROCESS_QUERY_LIMITED_INFORMATION)) ret
_s.all(1000); _i=1000
if(GetApplicationUserModelId(hp &_i +_s)) ret
appID.ansi(_s)
if(flags&1) appID-"shell:AppsFolder\"
ret 1
err+
