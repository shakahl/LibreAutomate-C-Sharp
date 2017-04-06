 \test dialog as attached toolbar
function

 Dialog that works like a QM toolbar attached to a window.
 Can be used in exe. QM toolbars can't.

 Edit this function: replace strings in the sub.InitToolbar line, add your code under case 1000 etc, edit the siz line.
 Create your imagelist with QM imagelist editor. Look in floating toolbar -> More Tools.
 If want to remove title bar, open this in Dialog Editor and remove style WS_CAPTION.

 To run, use mac() or a window trigger (not in exe). Don't call as function.
 In exe use mac(). The main exe thread must not exit until the toolbar is closed.


int hwndOwner=TriggerWindow
 int hwndOwner=win("Notepad" "Notepad")
 int hwndOwner=id(15 "Notepad")

str dd=
 BEGIN DIALOG
 0 "" 0x80C80040 0x8000180 0 0 223 15 "Toolbar"
 3 ToolbarWindow32 0x54010000 0x0 0 0 223 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

int hDlg=ShowDialog(dd &sub.DlgProc 0 hwndOwner 1)

siz 200 50 hDlg
sub.Attach hDlg hwndOwner 100 0

MessageLoop


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	 sub.InitToolbar id(3 hDlg) "One[]Two[]Three" "$qm$\il_qm.bmp" TBSTYLE_FLAT|TBSTYLE_LIST|CCS_NODIVIDER ;;buttons with text at the right
	sub.InitToolbar id(3 hDlg) "One[][1]Two[]Three" "$qm$\il_qm.bmp" CCS_NODIVIDER 1 ;;buttons without text, with tooltips. Button "Two" is with text.
	SetTimer hDlg 30 500 0
	
	case WM_TIMER
	sel wParam
		case 30
		sub.Attach hDlg GetParent(hDlg) 100 0
	
	case WM_DESTROY
	PostQuitMessage 0
	
	case WM_MOUSEACTIVATE
	ret DT_Ret(hDlg MA_NOACTIVATE)
	
	case WM_CONTEXTMENU
	sel ShowMenu("1About[]100Close" hDlg)
		case 1 mes "About"
		case 100 clo hDlg
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 1000
	out "One" ;;replace these out with run, mac or other code
	case 1001
	out "Two"
	case 1002
	out "Three"
ret 1


#sub Attach
function hDlg hwndOwner x y

 Attaches toolbar to owner window:
   If owner closed, closes toolbar.
   If owner hidden, hides toolbar.
   If owner unhidden, unhides toolbar.
   If owner moved, moves toolbar.

 hDlg - toolbar window.
 hwndOwner - owner window.
 x y - toolbar position relative to owner window.


if !IsWindow(hwndOwner)
	clo hDlg
	ret

int h=GetAncestor(hwndOwner 2)

if !IsWindowVisible(h) or (hwndOwner!h and (!IsWindowVisible(hwndOwner) or IsIconic(hwndOwner)))
	if(IsWindowVisible(hDlg)) hid hDlg
else if !IsIconic(h)
	RECT ro rd; GetWindowRect hwndOwner &ro; GetWindowRect hDlg &rd
	x+ro.left; y+ro.top
	if(x!rd.left or y!rd.top) mov x y hDlg
	if(!IsWindowVisible(hDlg))
		int wp(GetWindow(h GW_HWNDPREV)) fl(SWP_SHOWWINDOW|SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOOWNERZORDER)
		if(wp=hDlg) wp=0; fl|SWP_NOZORDER
		SetWindowPos hDlg wp 0 0 0 0 fl


#sub InitToolbar
function htb $buttons $imagelist tbStyle [flags] ;;flags: 1 tooltips instead of text

 Adds toolbar buttons.

 htb - child toolbar control.
 buttons - list of button labels. Multiline.
 imagelist - imagelist bitmap file. Create it with QM imagelist editor. Look in floating toolbar -> More Tools.
 tbStyle - styles to add to the child toolbar control. Documented in MSDN library.
 flags:
   1 - don't show button text, instead show a tooltip with the text.
      Text can be displayed for some buttons by inserting character code 1 at the beginning of button text. Example: "Text in tooltip[][1]Text on button".
      Also adds styles TBSTYLE_LIST|TBSTYLE_TOOLTIPS and sets extended style TBSTYLE_EX_MIXEDBUTTONS.


if(flags&1) tbStyle|TBSTYLE_LIST|TBSTYLE_TOOLTIPS
SetWinStyle htb tbStyle 1

__ImageList-- t_il.Load(imagelist)
SendMessage htb TB_SETIMAGELIST 0 t_il

ARRAY(str) as=buttons
ARRAY(TBBUTTON) a.create(as.len)
int i
for i 0 a.len
	TBBUTTON& t=a[i]
	t.idCommand=1000+i
	t.iBitmap=i
	lpstr s=as[i]; if(s[0]=1) s+1; t.fsStyle|BTNS_SHOWTEXT
	t.iString=SendMessage(htb TB_ADDSTRINGW 0 @_s.fromn(s -1 "" 1)) ;;note: the string must be terminated with two 0
	t.fsState=TBSTATE_ENABLED

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(htb TB_ADDBUTTONS a.len &a[0])

if(flags&1) SendMessage(htb TB_SETEXTENDEDSTYLE 0 TBSTYLE_EX_MIXEDBUTTONS)
