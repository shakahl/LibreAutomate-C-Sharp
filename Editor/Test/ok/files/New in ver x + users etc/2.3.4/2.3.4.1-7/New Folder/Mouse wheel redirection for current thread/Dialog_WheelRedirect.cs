\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Redirects WM_MOUSEWHEEL messages of this thread to the control from mouse.

 out
int- t_hh_wheel=SetWindowsHookEx(WH_GETMESSAGE &HookProc_WheelRedirect 0 GetCurrentThreadId)

str controls = "3 4"
str e3 lb4
e3="a[]b[]c[]d[]e[]f[]g[]h"
lb4="&a[]b[]c[]d[]e[]f[]g[]h"
if(!ShowDialog("" &Dialog_WheelRedirect &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54231044 0x200 0 0 96 26 ""
 4 ListBox 0x54230101 0x200 0 28 96 26 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
