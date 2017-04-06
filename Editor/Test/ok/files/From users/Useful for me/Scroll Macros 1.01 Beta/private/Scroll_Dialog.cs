 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile Scroll_Def

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 158 127 "Form"
 1 Button 0x54030001 0x4 16 108 48 14 "OK"
 40 Edit 0x54032000 0x204 114 10 18 14 ""
 41 Edit 0x54032000 0x204 114 34 18 14 ""
 2 Button 0x54030000 0x4 84 108 48 14 "Cancel"
 3 Button 0x54032009 0x4 0 0 30 12 "&1"
 4 Button 0x54002009 0x4 0 10 30 12 "&2"
 5 Button 0x54032009 0x4 0 20 30 13 "&3"
 6 Button 0x54002009 0x4 0 30 30 12 "&4"
 7 Button 0x54032009 0x4 0 40 30 12 "&5"
 8 Button 0x54002009 0x4 0 50 30 12 "&6"
 9 Button 0x54032009 0x4 0 60 30 13 "&7"
 10 Button 0x54002009 0x4 0 70 30 12 "&8"
 11 Button 0x54032009 0x4 0 80 30 12 "&9"
 12 Button 0x54002009 0x4 0 90 30 12 "10 &a"
 13 Button 0x54032009 0x4 38 0 30 12 "11 &b"
 14 Button 0x54002009 0x4 38 10 30 12 "12 &c"
 15 Button 0x54032009 0x4 38 20 30 13 "13 &d"
 16 Button 0x54002009 0x4 38 30 30 12 "14 &e"
 17 Button 0x54032009 0x4 38 40 30 12 "15 &f"
 18 Button 0x54002009 0x4 38 50 30 12 "16 &g"
 19 Button 0x54032009 0x4 38 60 30 13 "17 &h"
 20 Button 0x54002009 0x4 38 70 30 12 "18 &i"
 21 Button 0x54032009 0x4 38 80 30 12 "19 &j"
 22 Button 0x54002009 0x4 38 90 30 12 "20 &k"
 23 Button 0x54032009 0x4 76 0 30 12 "21 &l"
 24 Button 0x54002009 0x4 76 10 30 12 "22 &m"
 25 Button 0x54032009 0x4 76 20 30 13 "23 &n"
 26 Button 0x54002009 0x4 76 30 30 12 "24 &o"
 27 Button 0x54032009 0x4 76 40 30 12 "25 &p"
 28 Button 0x54002009 0x4 76 50 30 12 "26 &q"
 29 Button 0x54032009 0x4 76 60 30 13 "27 &r"
 30 Button 0x54002009 0x4 76 70 30 12 "28 &s"
 31 Button 0x54032009 0x4 76 80 30 12 "29 &t"
 32 Button 0x54002009 0x4 76 90 30 12 "30 &u"
 33 Button 0x54002009 0x4 114 50 30 12 "1 &v"
 34 Button 0x54032009 0x4 114 60 30 13 "2 &w"
 35 Button 0x54002009 0x4 114 70 30 12 "3 &x"
 36 Button 0x54032009 0x4 114 80 30 12 "4 &y"
 37 Button 0x54002009 0x4 114 90 30 12 "5 &z"
 38 Static 0x54020000 0x4 114 26 48 13 "    Page"
 39 Static 0x54020000 0x4 114 2 48 12 "<-- Line"
 END DIALOG
 DIALOG EDITOR: "" 0x2010500 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
if(ctrlid<33 && ctrlid>2) _m.scrollFactor=ctrlid-2; DT_Ok hDlg
else if(ctrlid<38 && ctrlid>32) _m.scrollFactor=(ctrlid-32)*-1; DT_Ok hDlg
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
