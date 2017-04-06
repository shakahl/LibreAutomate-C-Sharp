\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("opt_dlg" &opt_dlg 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 231 162 "Form"
 2 Button 0x54030001 0x4 182 148 48 14 "Cancel"
 3 Button 0x54032000 0x4 8 50 48 14 "end ''errror''"
 4 Button 0x54032000 0x4 58 50 88 14 "end ''error'' su opt end 1"
 5 Button 0x54032000 0x4 8 68 48 14 "shutdown -7"
 6 Button 0x54032000 0x4 58 68 54 14 "shutdown -7 1"
 8 Button 0x54032000 0x4 60 88 80 14 "wait su opt waitmsg 1"
 9 Button 0x54032000 0x4 8 88 48 14 "wait normal"
 10 Button 0x54032000 0x4 8 14 22 14 "end"
 11 Button 0x54032000 0x4 34 14 68 14 "end su opt end 1"
 7 Button 0x54032000 0x0 8 32 48 14 "QM error"
 12 Button 0x54032000 0x0 58 32 86 14 "QM error su opt end 1"
 END DIALOG
 DIALOG EDITOR: "" 0x2010505 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 12 opt end 1; act "ddddddddddddddd"
	case 7 act "ddddddddddddddd"
	case 11 opt end 1; end
	case 10 end
	case 9 wait 5
	case 8 opt waitmsg 1; wait 5
	case 6 shutdown -7 1
	case 5 shutdown -7
	case 4 opt end 1; end "error"
	case 3 end "error"
	
ret 1
