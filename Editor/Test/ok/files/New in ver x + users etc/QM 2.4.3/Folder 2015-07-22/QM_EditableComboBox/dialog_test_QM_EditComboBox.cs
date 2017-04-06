\Dialog_Editor
 /exe 1
out
init_QM_ComboBox
str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 224 150 "Dialog"
 3 QM_EditComboBox 0x54030080 0x00200 8 8 96 12 "cb1"
 6 ComboBox 0x54230242 0x0 8 28 96 213 ""
 9 QM_EditComboBox 0x54031244 0x4200 8 44 96 48 "cb2"
 5 ComboBox 0x54230243 0x0 108 8 96 213 ""
 4 Edit 0x54030080 0x200 108 44 96 13 ""
 7 Button 0x54012003 0x0 108 28 48 10 "Disable"
 18 Edit 0x54231044 0x200 8 96 96 48 ""
 8 Button 0x54032000 0x0 168 76 48 14 "Test"
 10 Button 0x54012003 0x0 164 28 48 10 "Readonly"
 1 Button 0x54030001 0x4 116 132 48 14 "OK"
 2 Button 0x54030000 0x4 168 132 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""
 3 QM_EditComboBox 0x54030080 0x200 8 8 96 12 "0"

 def WS_CAPTION       0xC00000
 def WS_BORDER        0x800000
 def WS_DLGFRAME      0x400000
 def WS_THICKFRAME    0x40000 ;;resizable
 def WS_EX_DLGMODALFRAME     0x00000001
 def WS_EX_WINDOWEDGE        0x00000100
 def WS_EX_CLIENTEDGE        0x00000200
 def WS_EX_STATICEDGE        0x00020000

 def WS_VSCROLL 0x200000
 def WS_HSCROLL 0x100000

 def WS_EX_LAYOUTRTL 0x00400000
 def WS_EX_LEFTSCROLLBAR 0x00004000
 def WS_EX_RIGHT 0x00001000
 def WS_EX_RTLREADING 0x00002000


 __ImageList il.Load("$qm$\il_qm.bmp")
 __ImageList il
 if(1) il=__ImageListLoad("$qm$\il_qm.bmp")
 else il=ImageList_Create(1 16 0 0 0) ;;set row height. Without this on XP 14, with icons 18; on Win7+ always 19.

str controls = "3 6 9 5 4 7 18 10"
str qmec3cb cb6 qmec9cb cb5 e4 c7Dis e18 c10Rea
 qmec3cb=",,7[]one,,1[]two[]Three,,1[]Four[]Thr"
 qmec3cb=",,1,Cue,0x8000,0xf0ffff[]one,,1,Tooltip,0xff,0xc0f0c0,4[]Header,,0x100[]Three,,1,''Multiline[]tooltip'',,,2[]Four[]Thr wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww,,,Tooooooooo"
qmec3cb=
 ,$qm$\il_qm.bmp,,Cue
 one,11,0xE,Tooltip,0xff,0xc0f0c0
 Header,-1,0xE,,,0xe0e0e0
 " Three",3,1,"Multiline
 tooltip"
 Four,4,,,,,1
 Middle,-1,0xE,,,0xe0e0e0
 "Thr ""wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww""",5,,Tooooooooo
 Footer,-1,0xE,,,0xe0e0e0

 Header,-1,0xE,,,0xe0e0e0

 qmec3cb=",$qm$\il_qm.bmp,1[]one,,1,Tooltip,0xff,0xc0f0c0,4[]two[]Three,,1,''Multiline[]tooltip''[]Four[]Thr wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww,,,Tooooooooo"
 qmec3cb="0x5,,3[]Mercury[]Venus[]Earth"
 qmec3cb="simple text"
 qmec3cb=",,[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3"
 rep(10000) qmec3cb.addline(_s.RandomString(5 15 "a-zA-Z") 1)

 qmec3cb="0,,[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3"
 qmec3cb="edit text,,[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3"
 qmec3cb="0,$qm$\il_qm.bmp,[]one,1[]two iiiiiiiiiiiiiiiiWW,2,1[]three WWWWWWWWWWWWWWii,3[]default icon"
 qmec3cb="1,$qm$\il_qm.bmp,3[]one,1[]two iiiiiiiiiiiiiiiiWW,2,1[]three WWWWWWWWWWWWWWii,3[]default icon"
 qmec3cb="0,,1[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3[]default icon"
 qmec9cb="0x1,$qm$\il_qm.bmp,3[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3[]default icon"
 qmec9cb=",$qm$\il_qm.bmp,1[]one,1[]two,2,1[]three loooooooooooooooooooooooooooooooooooooooong,3[]default icon"
qmec9cb="38"
int i; for(i 0 40) qmec9cb.addline(F"item {i}" 1)
qmec9cb.addline("LAST" 1)
 out "'%s'" qmec9cb
 qmec9cb=qmec3cb
 qmec9cb.getmacro("")
 qmec9cb="single line"
 qmec9cb="multi line[]"
 qmec9cb="''multi[]line[]''"
cb6="one[]two[]Three[]four[]five[]Thr[]loooooooooooooooooooooooooooooooooooong"
 rep(5) cb6.addline(cb6 1)
cb5="&one[]two[]Three[]four[]five[]Thr[]loooooooooooooooooooooooooooooooooooong"

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qmec3cb


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 DT_SetAutoSizeControls hDlg "3sh 9s"
	 SetWindowTheme id(6 hDlg) L"" L""
	__Font- f.Create("Comic Sans MS" 16)
	 f.SetDialogFont(hDlg "3 6 4 9")
	
	 SetWindowTheme id(6 hDlg) L"" L""
	 SendMessage id(3 hDlg) EM_SETCUEBANNER 0 @" Window"
	
	 PF
	 int h=CreateControl(0x00000200 "QM_EditComboBox" 0 0x54030080 0 70 100 30 hDlg 77)
	 PN;PO
	 outw h
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_PAINT
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	RECT r; SetRect &r 20 100 50 130
	FillRect ps.hDC &r GetSysColorBrush(COLOR_INFOBK)
	
	int htheme=OpenThemeData(hDlg L"ComboBox")
	 out htheme
	DrawThemeBackground(htheme ps.hDC CP_DROPDOWNBUTTON CBXS_NORMAL &r &ps.rcPaint) ;;gray
	 DrawThemeBackground(htheme ps.hDC CP_DROPDOWNBUTTONRIGHT CBXS_NORMAL &r &ps.rcPaint) ;;transparent, same CP_DROPDOWNBUTTONLEFT
	 DrawThemeBackground(htheme ps.hDC CP_DROPDOWNBUTTON CBXS_HOT &r &ps.rcPaint) ;;blue
	CloseThemeData htheme
	
	EndPaint hDlg &ps
ret
 messages2
sel wParam
	case 8 ;;Test
	 SetWinStyle id(9 hDlg) 0x200 2
	 _s="bad[]csv''"; _s.setwintext(id(9 hDlg))
	 SetFocus id(6 hDlg)
	 SetDlgItemText hDlg 6 "two"
	 SendMessage id(6 hDlg) WM_SETTEXT 0 "two"
	
	 str s="-1"; rep(1000) s.addline("Mmmmmmmmmmmmmmmmm")
	 int h1=id(6 hDlg)
	 int h2=id(3 hDlg)
	 PF
	 DT_SetControl h1 0 s
	 PN
	 DT_SetControl h2 0 s
	 PN
	 PO
	
	_s.setwintext(id(3 hDlg))
	
	case 7 ;;Disable
	EnableWindow id(3 hDlg) !but(lParam)
	
	case 10 ;;Readonly
	SendMessage id(3 hDlg) EM_SETREADONLY but(lParam) 0
	
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog162
 exe_file  $my qm$\Dialog162.qmm
 flags  6
 guid  {7E03E342-7321-4D29-AFAA-1B30FEE92E1B}
 END PROJECT
