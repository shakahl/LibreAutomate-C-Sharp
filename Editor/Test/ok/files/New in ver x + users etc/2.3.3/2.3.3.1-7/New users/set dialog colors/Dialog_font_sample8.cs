\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 3"
str e4 e3
e3="one[]two[]three[]four[]five[]six[]seven"
if(!ShowDialog("" &Dialog_font_sample8 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A40 0x100 0 0 167 79 "Dialog Fonts"
 4 Edit 0x54030080 0x200 136 36 18 16 ""
 3 Edit 0x54230844 0x20000 14 26 94 42 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	DT_SetTextColor hDlg 0xff
	 DT_SetBackgroundColor hDlg 0 0xff8080
	DT_SetBackgroundColor hDlg 2 0xff8080 0xffffff
	 DT_SetBackgroundImage hDlg "$qm$\il_icons.bmp"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 12
	 DT_SetBackgroundImage hDlg "" 1
	 ARRAY(int) a; child "" "" hDlg 0 0 0 a; _s-"jk"; for(_i 0 a.len) _s.setwintext(a[_i])
	 ret
	Q &q
	RedrawWindow hDlg 0 0 RDW_INVALIDATE|RDW_ERASE|RDW_UPDATENOW
	Q &qq
	outq
ret 1