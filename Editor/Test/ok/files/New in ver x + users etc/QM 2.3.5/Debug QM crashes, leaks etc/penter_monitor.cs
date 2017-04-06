 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

__SharedMemory+ g_sm_profiling.Create("QM_sm_profiling" 4096)

str controls = "3"
str e3
if(!ShowDialog("penter_monitor" &penter_monitor &controls 0 128)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 104 15 "_penter monitor"
 3 Edit 0x54030080 0x200 0 0 68 14 ""
 4 Button 0x54032000 0x0 70 0 34 14 "Copy"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	Tray-- t.AddIcon("" "_penter monitor" 1 hDlg)
	
	case WM_TIMER
	sel wParam
		case 1
		int* m=g_sm_profiling.mem
		_s=F"0x{*m}"
		_s.setwintext(id(3 hDlg))
	
	case WM_USER+101
	sel lParam
		case WM_LBUTTONUP
		act hDlg; err
		SetTimer hDlg 1 1000 0
		
		case WM_RBUTTONUP
		sel ShowMenu("1 Exit" hDlg)
			case 1 DT_Cancel hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	_s.getwintext(id(3 hDlg))
	_s.setclip
	
	case IDCANCEL
	KillTimer hDlg 1
	hid hDlg
	ret
ret 1

 BEGIN PROJECT
 main_function  penter_monitor
 exe_file  $my qm$\penter_monitor.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 on_before  MakeExeCloseRunning
 flags  14
 guid  {76A830B7-345B-4DF9-BC72-60B4C47056AC}
 END PROJECT
