 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str c4Goo
if(!ShowDialog("Dialog148" &Dialog148 &controls)) ret
 if(!ShowDialog("Dialog148" &Dialog148 0 0 128)) ret
out c4Goo

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 88 24 48 14 "Button" "tooooo"
 4 Button 0x54012003 0x0 120 52 52 14 "Goo"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 5 Button 0x54020007 0x0 84 8 108 100 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040104 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 DT_SetBackgroundColor hDlg 1 0xff0000 0x8000
	 DT_SetBackgroundImage hDlg "$my qm$\copy.gif"
	
	 SetTimer hDlg 1 2000 0
	 case WM_TIMER
	 KillTimer hDlg 1
	 hid- hDlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	 ARRAY(int) a
	 GetThreadWindows a
	 int i
	 for i 0 a.len
		 outw a[i]
ret 1

 BEGIN PROJECT
 main_function  Dialog148
 exe_file  $my qm$\Dialog148.qmm
 flags  6
 guid  {E59E0DF2-D281-4690-8C94-829B48184D69}
 END PROJECT
