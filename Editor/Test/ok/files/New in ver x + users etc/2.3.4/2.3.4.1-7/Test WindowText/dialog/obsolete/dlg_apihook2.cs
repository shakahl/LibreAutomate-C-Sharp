 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
int w=win("dlg_apihook" "#32770"); if(w) clo w

#compile "DAH_def"
CApiHook- t_ah
 t_ah.Hook

int- t_out t_all=0
int hwndqm=win("" "QM_Editor")

str controls = "5 6"
str e5 qmg6x
e5="abcd Ԏݭῼ"
qmg6x="C,D"

str dd1=
 BEGIN DIALOG
 1 "" 0x90CC0AC8 0x0 0 0 225 494 "dlg_apihook"
 3 Button 0x50032000 0x0 154 6 48 14 "Buttonn"
 4 Static 0x54000000 0x0 154 36 48 13 "Text"
 5 Edit 0x54030080 0x200 154 52 46 14 ""
 6 QM_Grid 0x56031041 0x0 154 76 52 42 "0x0,0,0,0,0x0[]A,,,[]B,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""
 3 Button 0x54032000 0x0 154 6 48 14 "Buttonn"


str dd2=
 BEGIN DIALOG
 1 "" 0x90CC0AC8 0x0 0 0 223 500 "dlg_apihook"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

int- t_more=1

#opt nowarnings 1
if(t_more) if(!ShowDialog(dd1 &dlg_apihook &controls hwndqm 0 0 0 0 -1 0 "dialog.ico" "dlg_apihook")) ret
else if(!ShowDialog(dd2 &dlg_apihook 0 hwndqm 0 0 0 0 -1)) ret

ret
 messages
 OutWinMsg message wParam lParam
 out message
sel message
	case WM_INITDIALOG
	out
	if(t_more) DAH_ChildDialogs(hDlg)
	 SetTimer hDlg 1 500 0
	 SetTimer hDlg 2 500 0
	
	 rep 3
		 mac "DAH_OtherThread"
	
	case WM_TIMER
	sel wParam
		case 2 mac "DAH_OtherThread"
		
		case 1
		KillTimer hDlg 1
		 g1
		
		 out
		 rep 100
			 t_ah.Hook
			 t_ah.Unhook
			  0.001
			 0.01
		 out "ok"
		 ret
		
		t_out=1
		 Q &q
		 outb &ExtTextOutW 16
		t_ah.Hook
		 outb &ExtTextOutW 16
		 Q &qq
		RedrawWindow hDlg 0 0 RDW_ERASE|RDW_FRAME|RDW_INVALIDATE|RDW_UPDATENOW|RDW_ALLCHILDREN
		 Q &qqq
		t_ah.Unhook
		 outb &ExtTextOutW 16
		 Q &qqqq
		 outq
		t_out=0
	case WM_APP
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_PAINT
	 t_out=1
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	DAH_DrawMain ps.hDC
	 DAH_BitBlt ps.hDC
	 DAH_BufferedPaint ps.hDC
	EndPaint hDlg &ps
	 t_out=0
ret
 messages2
sel wParam
	case IDOK
	 case 3 goto g1
	case 3 BringWindowToTop lParam
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0 0 Co
	 -
	 >&Recent
		 Empty : 102 0 3
		 <
	 <
 >&Edit
	 Cu&t : 103 0 0 Cx
	 -
	 Select &All : 104 0 0 Ca
	 <
 &Help : 105 0
 END MENU
