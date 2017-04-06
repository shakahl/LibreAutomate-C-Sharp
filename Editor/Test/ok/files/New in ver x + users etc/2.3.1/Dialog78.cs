\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 7"
str cb3 lb7
cb3="&ab[]cd"
if(!ShowDialog("Dialog78" &Dialog78 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 164 90 48 14 "OK"
 2 Button 0x54030000 0x4 164 114 48 14 "Cancel"
 4 Button 0x54032000 0x0 40 78 54 14 "focus combo"
 5 Button 0x54032000 0x0 6 96 126 14 "get combo text without focusing it"
 6 Button 0x54032000 0x0 6 116 126 14 "get combo item 1 text"
 3 ComboBox 0x54230641 0x0 8 22 96 48 ""
 7 ListBox 0x54230101 0x200 122 22 96 48 ""
 8 Static 0x54000000 0x0 32 6 48 12 "Box A"
 9 Static 0x54000000 0x0 142 6 48 12 "Box B"
 END DIALOG
 DIALOG EDITOR: "" 0x203000D "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	act id(3 hDlg)
	
	case 5
	out _s.getwintext(id(3 hDlg))
	
	case 6
	CB_GetItemText(id(3 hDlg) 1 _s); out _s
	
	case IDOK
	case IDCANCEL
ret 1





















 \Dialog_Editor
 function# hDlg message wParam lParam
 if(hDlg) goto messages
 
 str controls = "3"
 str cb3
 cb3="&ab[]cd"
 if(!ShowDialog("Dialog78" &Dialog78 &controls)) ret
 
  BEGIN DIALOG
  0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
  1 Button 0x54030001 0x4 120 116 48 14 "OK"
  2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
  3 ComboBox 0x54230242 0x0 16 12 96 213 ""
  4 Button 0x54032000 0x0 34 36 54 14 "focus combo"
  5 Button 0x54032000 0x0 6 56 126 14 "get combo text without focusing it"
  6 Button 0x54032000 0x0 6 76 126 14 "get combo item 1 text"
  END DIALOG
  DIALOG EDITOR: "" 0x2030103 "" "" ""
 
 ret
  m essages
 sel message
	 case WM_INITDIALOG
	 case WM_DESTROY
	 case WM_COMMAND goto messages2
 ret
  m essages2
 sel wParam
	 case 4
	 act id(3 hDlg)
	 
	 case 5
	 out _s.getwintext(id(3 hDlg))
	 
	 case 6
	 CB_GetItemText(id(3 hDlg) 1 _s); out _s
	 
	 case IDOK
	 case IDCANCEL
 ret 1
