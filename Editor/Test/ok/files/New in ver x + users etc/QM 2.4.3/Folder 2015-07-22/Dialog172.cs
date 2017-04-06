\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 222 136 "Dialog"
 3 QM_ComboBox 0x54030242 0x0 28 38 96 13 ""
 4 Button 0x54012003 0x0 112 8 48 10 "Check"
 5 Button 0x54032000 0x0 28 8 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str il="$qm$\il_qm.bmp"
 str il="$qm$\empty.ico[]$qm$\cut.ico[]$qm$\copy.ico[]$qm$\paste.ico"
 __ImageList il.Load("$qm$\il_qm.bmp")

str controls = "3 4"
str qmcb3 c4Che

qmcb3=F"0,''{il}'',0x10,Cue,,,200[]zero,1,[]one,2,1[]two,3[]three ABCDEFGHIJKLMNOP,4,,''Tooltip wwwwwwwwwwwwwwww[]mmm sjdhajhdjashjdajjsakjdhkja''[]four"
 qmcb3=F"1|2,''{il}'',0x002,Cue,,,-180,,-88[]zero,-1,4|2|8[]one,2,1[]two,3[]three ABCDEFGHIJKLMNOP,4"
rep(150) qmcb3.addline(_s.RandomString(5 20 "a-z") 1);; qmcb3+",4"

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qmcb3


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	__Font- f.Create("Tahoma" 20)
	 f.SetDialogFont(hDlg "3")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
 OutWinMsg message wParam lParam 0 lParam
sel wParam
	case 5
	int h=id(3 hDlg)
	 out SendMessage(h CB_GETITEMDATA 2 0)
	 out SendMessage(h CB_SETITEMDATA 2 5)
	 out SendMessage(h CB_GETITEMDATA 2 0)
	 out SendMessage(h CB_SETITEMDATA 2 4)
	 CB_Add(h "new")
	 DT_GetControl hDlg 3 _s; out _s
	 out SendMessage(h CB_GETDROPPEDSTATE 0 0)
	 out SendMessage(h CB_DELETESTRING 1 0)
	 out SendMessageW(h CB_INSERTSTRING 1 L"new")
	 out SendMessageW(h CB_RESETCONTENT 0 0)
	 out SendMessageW(h CB_GETCOUNT 0 0)
	 out SendMessageW(h CB_GETCURSEL 0 0)
	 out SendMessageW(h CB_GETCURSEL 0 0)
	 out SendMessageW(h CB_GETLBTEXTLEN 2 0)
	 CB_GetItemText(h 2 _s); out _s
	 out SendMessageW(h CB_SHOWDROPDOWN 1 0)
	 out SendMessageW(h CB_SETEDITSEL 0 5)
	 out SendMessageW(h CB_LIMITTEXT 5 5)
	 out SendMessageW(h CB_GETITEMHEIGHT -1 0)
	 out SendMessageW(h CB_SETITEMHEIGHT -1 40)
	 out SendMessageW(h LB_SETSEL 0 1)
	 out SendMessageW(h LB_GETSEL 1 0)
	 DT_GetControl hDlg 3 _s; out _s
	case IDCANCEL
ret 1
