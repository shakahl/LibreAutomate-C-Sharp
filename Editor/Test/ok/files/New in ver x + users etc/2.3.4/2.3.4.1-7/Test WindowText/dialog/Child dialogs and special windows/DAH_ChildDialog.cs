 /dlg_apihook
\Dialog_Editor
function# hDlg x y cx cy [$name]
if(empty(name)) name="Dialog"

DAH_SetWindowRegion hDlg 60 30 30 70

int es
 es|WS_EX_LAYOUTRTL
 es|WS_EX_NOINHERITLAYOUT
 es|WS_EX_RIGHT ;;ok. Just some windows align text on right.
 es|WS_EX_RTLREADING ;;has no effect in my shell language. Not tested with other languages.

str dd=
F
 BEGIN DIALOG
 0 "" 0x10CF0048 {es} {x} {y} {cx} {cy} "{name}"
 4 Edit 0x54030080 0x200 30 14 26 16 ""
 3 Button 0x54012003 0x0 8 10 48 12 "CheckABC"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""
 5 Static 0x54000003 0x0 8 38 16 16 ""

str controls = "4 3"
str e4 c3Che
 si5="$qm$\macro.ico"
int h=ShowDialog(dd 0 &controls hDlg 1 WS_CHILD|WS_CLIPSIBLINGS WS_POPUP 0 x y)
 int h=ShowDialog(dd &DAH_ChildDialogProc &controls hDlg 1 WS_CHILD|WS_CLIPSIBLINGS WS_POPUP 0 x y)
BringWindowToTop h


 __GdiHandle hw2=CreateRectRgn(0 0 0 0)
 if GetWindowRgn(h hw2)
	 int he2=CreateEllipticRgn(30 20 40 60)
	 CombineRgn(he2 hw2 he2 RGN_DIFF)
	 SetWindowRgn(h he2 0)

ret h
