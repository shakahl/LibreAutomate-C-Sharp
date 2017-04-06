\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "9 6 7 8 13 15 14 18 20"
str c9Che e6 e7 cb8 lb13 o15Opt sb14 si18 e20
 sb14="$qm$\il_icons.bmp"
sb14="$qm$\bmp00003.bmp"
si18="&$qm$\qm.exe,4"
e6="aaaaaa"
e7=e6
cb8="&aaaaaaaaaaaaaa[]bbbbbbbbbbbbbbbbb"
lb13="aaaaa"
if(!ShowDialog("Dialog_font_sample5" &Dialog_font_sample5 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A40 0x100 0 0 167 164 "Dialog Fonts"
 9 Button 0x54012003 0x0 112 36 48 13 "Check"
 4 Static 0x54000000 0x4 112 22 46 10 "Normal"
 3 Static 0x54000000 0x0 60 6 42 12 "Normal"
 5 Static 0x54000000 0x20 112 6 48 12 "Transparent"
 6 Edit 0x54030080 0x200 6 66 96 14 ""
 7 Edit 0x54230844 0x20000 6 52 96 13 ""
 8 ComboBox 0x54230243 0x0 6 82 96 213 ""
 11 QM_Splitter 0x54030000 0x0 104 6 6 150 ""
 17 QM_Splitter 0x54030000 0x0 0 6 4 148 ""
 12 Button 0x5403A000 0x0 54 100 48 14 "Text"
 13 ListBox 0x54230101 0x200 112 100 50 21 ""
 15 Button 0x54032009 0x0 112 52 48 13 "Option"
 14 Static 0x5400000E 0x0 112 124 2 2 ""
 18 Static 0x54000003 0x20 112 140 16 16 ""
 20 Edit 0x54231044 0x200 8 116 94 29 ""
 19 Static 0x54000010 0x20000 6 148 93 2 ""
 16 Button 0x54020007 0x0 6 22 96 18 "aaaaaaaaaaaaaaaaaaa"
 10 Button 0x54020007 0x0 112 70 44 26 "Sssssss"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""
 1 Button 0x54030001 0x4 4 82 48 14 "OK"
 2 Button 0x54030000 0x4 56 82 48 14 "Cancel"

ret
 messages
sel message
	case WM_INITDIALOG
	out
	TO_NoTheme id(12 hDlg) ;;Button
	 TO_NoTheme id(9 hDlg) ;;Check
	__Font-- f
	f.Create("Courier New" 14 1)
	f.SetDialogFont(hDlg "3 4 5")
	 f.SetDialogFontColor(hDlg 0xff "3 4 5 6 7 8 9 10 11 13 15")
	
	DT_SetTextColor hDlg 0xff
	DT_SetTextColor hDlg 0xff00 "3 5"
	 DT_SetBackgroundColor hDlg 0 0xff8080
	DT_SetBackgroundColor hDlg 1 0xff8080 0xffffff
	 DT_SetBackgroundImage hDlg "$qm$\il_icons.bmp"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORDLG
	 out 1
	 int color1(0xff8080) color2(0xffffff) vertical(0)
	 TRIVERTEX x1 x2; int R G B
	 RECT r; GetClientRect hDlg &r; memcpy &x2 &r.right 8
	 ColorToRGB color1 R G B; x1.Red=R<<8; x1.Green=G<<8; x1.Blue=B<<8
	 ColorToRGB color2 R G B; x2.Red=R<<8; x2.Green=G<<8; x2.Blue=B<<8
	 GRADIENT_RECT r1.LowerRight=1
	 GdiGradientFill wParam &x1 2 &r1 1 vertical
	 ret GetStockObject(NULL_BRUSH)
	
	 ret CustomColor
	
	 case WM_ERASEBKGND
	 ret DT_Ret(hDlg CustomColor)
	
	 case WM_CTLCOLORSTATIC
	 SetBkMode wParam 1
	 RECT r; GetClientRect lParam &r; FillRect wParam &r GetStockObject(GRAY_BRUSH)
	 ret GetStockObject(NULL_BRUSH)
	 
	 case WM_PAINT
	 PAINTSTRUCT ps
	 BeginPaint hDlg &ps
	 RECT rr; GetClientRect hDlg &rr; FillRect ps.hDC &rr GetStockObject(GRAY_BRUSH)
	 EndPaint hDlg &ps
ret
 messages2
sel wParam
	case 12
	 DT_SetBackgroundImage hDlg "" 1
	 ARRAY(int) a; child "" "" hDlg 0 0 0 a; _s-"jk"; for(_i 0 a.len) _s.setwintext(a[_i])
	 ret
	RedrawWindow hDlg 0 0 RDW_INVALIDATE|RDW_ERASE|RDW_UPDATENOW
ret 1