\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "18 3"
str si18 si3
si18="&$qm$\qm.exe,4"
si3="&$qm$"
if(!ShowDialog("" &Dialog_font_sample7 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A40 0x100 0 0 167 164 "Dialog Fonts"
 18 Static 0x54000003 0x0 74 124 16 16 ""
 3 Static 0x54000003 0x0 120 100 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""
 3 Static 0x54000000 0x0 74 108 48 13 "Text"

ret
 messages
sel message
	case WM_INITDIALOG
	out
	DT_SetTextColor hDlg 0xff
	 DT_SetBackgroundColor hDlg 0 0xff8080
	DT_SetBackgroundColor hDlg 1 0xff8080 0xffffff
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