\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 <ctype/lparam/image/overlayImage/stateImage/indent/ctypes>

str controls = "3"
str qmg3
qmg3=
 <0//-2///-1>edit noicon indent(-1),b,c
 <1/2/3/2//1>combo icon(3) ov(2) indent(1),b,c
 <2//5//8>check icon(5) st(8),Yes,c
 <7>read-only,b,c
 <8>edit multiline,b,c
 <9>combo sorted,b,c
 <16>edit+button,b,c
 <17>combo+button,b,c
 <//////-1 . 2>mixed,b,c
 <1//////-1 -1 2>mixed,b,c
 <1//////-1 . 2>mixed,b,c
 <//////-1 -1 2>mixed,b,c
 inherit,b,c
 
if(!ShowDialog("" &dlg_Grid_row_prop &controls _hwndqm)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 378 272 "Dialog"
 3 QM_Grid 0x54030000 0x0 0 0 378 250 "4,0,0,2[]A,50%,[]B,20%,[]C,20%,1,1[]"
 1 Button 0x54030001 0x4 2 256 48 14 "OK"
 2 Button 0x54030000 0x4 54 256 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	g.SetImagelist(il il)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
