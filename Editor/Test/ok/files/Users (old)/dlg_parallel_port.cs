\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 http://www.quickmacros.com/forum/viewtopic.php?f=4&t=3164
 http://logix4u.net/Legacy_Ports/Parallel_Port/Inpout32.dll_for_Windows_98/2000/NT/XP.html

typelib HWINTERFACELib {BD6BB71B-5C9A-4FB3-877E-8B513D28B951} 1.0

str controls = "8 10"
str e8 e10
e8=255
e10="0x0378"
if(!ShowDialog("dlg_parallel_port" &dlg_parallel_port &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Parallel Port"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 206 4 12 10 "HWINTERFACELib.Hwinterface {B9022892-EA92-4F94-8101-B9CDE30E667D}"
 6 Button 0x54032000 0x0 44 34 62 14 "send to output"
 7 Static 0x54000000 0x0 8 18 98 12 ""
 8 Edit 0x54030080 0x200 8 34 32 14 ""
 9 Static 0x54000000 0x0 10 78 34 12 "Port"
 10 Edit 0x54030080 0x200 46 76 32 14 ""
 5 Button 0x54020007 0x0 4 62 108 36 "Config"
 4 Button 0x54020007 0x0 4 4 106 50 "Input and output"
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "*" "" ""

ret
 messages
HWINTERFACELib.Hwinterface x._getcontrol(id(3 hDlg))
int port=val(_s.getwintext(id(10 hDlg)))

sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 50 0
	
	case WM_TIMER
	sel wParam
		case 1
		 x.OutPort(port+2 0x20)
		
		_s=x.InPort(port)
		_s-"input is: "
		_s.setwintext(id(7 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 6
	 x.OutPort(port+2 0)
	
	_s.getwintext(id(8 hDlg))
	x.OutPort(port val(_s))
	
	case IDOK
	case IDCANCEL
ret 1
