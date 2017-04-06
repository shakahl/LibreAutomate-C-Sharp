\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("DlgCopyPaste" &DlgCopyPaste &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54231044 0x200 2 16 218 100 ""
 4 Button 0x54032000 0x0 2 2 48 14 "Copy"
 5 Button 0x54032000 0x0 54 2 48 14 "Paste"
 6 Button 0x54032000 0x0 106 2 48 14 "Keys"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
 spe 100
sel wParam
	case 4 ;;Copy
	 opt waitmsg 1
	act id(3)
	key CH
	0.1
	int i
	for i 0 5
		 spe
		key HDSE
		 spe 100
		str s.getsel
		 str s.getsel(0 0 id(3))
		out s
		
	case 5 ;;Paste
	act id(3)
	for i 0 5
		outp "%i ffffffffffffffff[]" i
		 _s="jjjjjjjj[]"; _s.setsel(0 id(3))
	
	case 6 ;;Keys
	act id(3)
	for i 0 5
		key (i) ffffY
	
	case IDOK
	case IDCANCEL
	 case EN_CHANGE<<16|3 Sleep 500
ret 1
