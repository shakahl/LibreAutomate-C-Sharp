type G_TEST
	__Handle event
	int'Test
	int'Test_old
	str'sRandom

G_TEST+ g_test2751

g_test2751.Test=0
g_test2751.Test_old=0
g_test2751.sRandom="Random"
if(g_test2751.event=0) g_test2751.event=CreateEvent(0 0 0 0); else ResetEvent g_test2751.event

int ht=mac("sub.Fx_GUI")

out
rep
	if(2!=wait(0 HM ht g_test2751.event)) break ;;waits for 2 events: 1. Fx_GUI thread ended. 2. g_test2751.event set.
	
	g_test2751.sRandom.RandomString(5 5 "a-z")
	out F"{g_test2751.Test}: {g_test2751.sRandom}"
	g_test2751.Test_old=g_test2751.Test


#sub Fx_GUI

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 21 21 48 14 "Random + 0"
 4 Button 0x54032000 0x0 87 21 48 14 "+10"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
		SetTimer hDlg 1 500 0
	case WM_TIMER
		str s=F"{g_test2751.sRandom} + {g_test2751.Test}"
		s.setwintext(id(3 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
		g_test2751.Test+1
		SetEvent g_test2751.event
	case 4
		g_test2751.Test+10
		SetEvent g_test2751.event
	case IDOK
	case IDCANCEL
ret 1
