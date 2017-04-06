\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 6"
str e3 c4Act e6
c4Act=1
if(!ShowDialog("dialog_tooltip_properties" &dialog_tooltip_properties &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 6 6 96 14 "" "tooltip"
 4 Button 0x54012003 0x0 6 30 96 12 "Activate all tooltips"
 5 Static 0x54000000 0x0 6 48 30 12 "Time, s"
 6 Edit 0x54030080 0x200 40 46 62 14 ""
 7 Button 0x54032000 0x0 4 66 40 14 "Color..."
 8 Button 0x54032000 0x0 48 66 54 14 "Text color..."
 9 Button 0x54032000 0x0 4 84 98 14 "Set title and icon"
 10 Button 0x54032000 0x0 4 102 98 14 "Change tool info"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030501 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
__Tooltip& t=DT_GetTooltip(hDlg)
sel wParam
	case EN_CHANGE<<16|6 ;;set time
	_s.getwintext(lParam)
	SendMessage t.htt TTM_SETDELAYTIME TTDT_AUTOPOP val(_s 2)*1000 ;;also can set TTDT_INITIAL etc
	
	case 4 ;;activate/deactivate
	SendMessage t.htt TTM_ACTIVATE but(lParam) 0
	
	case [7,8] ;;color
	if(!ColorDialog(_i 0 hDlg)) ret
	if(GetWindowTheme(t.htt)) SetWindowTheme t.htt L"" L""
	SendMessage t.htt iif(wParam=7 TTM_SETTIPBKCOLOR TTM_SETTIPTEXTCOLOR) _i 0
	
	case 9 ;;title
	SendMessage t.htt TTM_SETTITLEW TTI_ERROR @"Title"
	
	case 10 ;;change some properties for a single control
	TOOLINFOW ti.cbSize=44; ti.hwnd=hDlg; ti.uId=id(3 hDlg); ti.uFlags=TTF_IDISHWND
	if(!SendMessage(t.htt TTM_GETTOOLINFOW 0 &ti)) ret
	ti.lpszText=@"new text"
	ti.uFlags|TTF_CENTERTIP
	SendMessage t.htt TTM_SETTOOLINFOW 0 &ti
	
ret 1

 Most these messages change properties for tooltips of all controls in dialog.
 If need only for some, use own __Tooltip variables, not DT_GetTooltip, and add tooltips in dialog procedure at run time, not in dialog editor.
 TTM_ messages are documented in MSDN Library.
