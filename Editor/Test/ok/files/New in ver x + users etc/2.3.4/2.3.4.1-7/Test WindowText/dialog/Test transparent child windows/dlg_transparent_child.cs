\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int+ g_atom_tc
if(!g_atom_tc) g_atom_tc=RegWinClass("transparent_child" &WndProc_transparent_child 4)

int w=win("Dialog Transparent Children" "#32770"); if(w) clo w; 0.2

if(!ShowDialog("dlg_transparent_child" &dlg_transparent_child 0 _hwndqm 0 0 0 0 -1 1)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 197 159 "Dialog Transparent Children"
 6 Button 0x54032000 0x0 114 4 48 14 "Button"
 3 transparent_child 0x54800000 0x0 2 2 96 48 ""
 7 Button 0x50012003 0x0 22 22 48 12 "Check1"
 4 transparent_child 0x54800000 0x0 2 54 96 48 ""
 8 Button 0x50012003 0x0 18 74 48 12 "Check2"
 5 transparent_child 0x54800000 0x20 18 108 80 48 ""
 9 Button 0x50012003 0x0 0 124 48 13 "Check3"
 10 SysTabControl32 0x54000040 0x0 102 56 96 48 ""
 11 Button 0x50012003 0x0 120 78 48 12 "Check4"
 14 Button 0x50012003 0x0 116 62 48 12 "Check6"
 12 Button 0x54020007 0x20 104 112 92 44 "Group"
 13 Button 0x50012003 0x0 118 136 48 12 "Check5"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 6
	goto g2
ret 1
 g1
SetWindowRgn(id(3 hDlg) CreateRectRgn(40 40 100 100) 0)
 ret
TO_TabAddTabs hDlg 10 "Tab"
SetProp id(10 hDlg) "some unique string" SubclassWindow(id(10 hDlg) &WndProc_transparent_child_subclass)
SetProp id(12 hDlg) "some unique string" SubclassWindow(id(12 hDlg) &WndProc_transparent_child_subclass)

ret
 g2
out
__GdiHandle hr=CreateRectRgn(0 0 0 0)
int w1=id(3 hDlg) ;;SetWindowRgn. Transparent to mouse. RealChildWindowFromPoint recognizes transparency.
int w2=id(4 hDlg) ;;WM_NCHITTEST. Transparent to mouse. RealChildWindowFromPoint does not recognize transparency.
int w3=id(5 hDlg) ;;WS_EX_TRANSPARENT. Opaque to mouse. Transparent or opaque depending on styles (this and siblings) and wndproc. RealChildWindowFromPoint does not recognize transparency.
int w4=id(10 hDlg) ;;tab control. Uses WM_NCHITTEST. Transparent to mouse. RealChildWindowFromPoint does not recognize transparency.
int w5=id(12 hDlg) ;;group button. Transparent to mouse. RealChildWindowFromPoint recognizes transparency. No region, no WM_NCHITTEST, don't know how Windows recognizes it.
int c1=id(7 hDlg)
int c2=id(8 hDlg)
int c3=id(9 hDlg)
int c4=id(11 hDlg)
int c5=id(13 hDlg)
 POINT p.x=37; p.y=45
 POINT p.x=47; p.y=47
 POINT p.x=34; p.y=128
 POINT p.x=187; p.y=135
 POINT p.x=183; p.y=231
int W(w2) C(c2)
POINT p; GetWinXY C p.x p.y 0 0 hDlg; p.x+1; p.y+2; out "%i %i" p.x p.y
POINT ps=p; ClientToScreen hDlg &ps
Q &q
int rcwfp=RealChildWindowFromPoint(hDlg p)
Q &qq
int ht=SendMessageW(W WM_NCHITTEST 0 MakeInt(ps.x ps.y))
Q &qqq
int rt=GetWindowRgn(W hr)
Q &qqqq
int _hild;;=child(p.x p.y hDlg 8)
Q &qqqqq
Acc _acc;;=acc(p.x p.y hDlg 1)
Q &qqqqqq
outq
outw rcwfp
out ht
out rt
outw _hild
if(_acc.a) out _acc.Name
