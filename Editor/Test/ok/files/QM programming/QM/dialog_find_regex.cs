
str dd=
 BEGIN DIALOG
 1 "" 0x90CE0AC8 0x80 0 0 438 64 "QM Find dialog text"
 3 RichEdit20W 0x54233044 0x200 0 0 438 46 ""
 4 Button 0x54012003 0x0 4 48 48 10 "Find 2" "Use the second Find dialog instance"
 5 Button 0x54032000 0x0 64 48 108 13 "Get text from Find dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3 4"
str re3 c4Fin
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	__Font-- t_f.Create("Courier New" 10); t_f.SetDialogFont(hDlg "3")
	DT_SetAutoSizeControls hDlg "3s 4mv 5mv"
	SendMessage id(3 hDlg) EM_SETEVENTMASK 0 ENM_CHANGE
	sub.Get(hDlg)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	sub.Set(hDlg)
	 SetFocus id(3 hDlg)
	
	case [4,5]
	_i=sub.Get(hDlg)
	if _i=0 and wParam=4 and but(lParam) ;;auto-show 'Find 2' dialog
		int w=win("Find" "#32770" "qm" 1); if(!w) w=_hwndqm
		Acc a.Find(w "PUSHBUTTON" "Clone this search (2 Find dialogs)" "class=ToolbarWindow32[]id=2057" 0x85)
		if(a.a) a.DoDefaultAction; err
	
ret 1


#sub FindWnd
function# hDlg
int w two=but(4 hDlg)
if two
	w=win("Find 2" "#32770" "qm" 1)
	if(!w) ret
else
	w=win("Find" "#32770" "qm" 1)
	if(!w) ret id(1127 _hwndqm)
ret id(1127 w)


#sub Get
function! hDlg

int- t_noSet

int c=sub.FindWnd(hDlg)
if(!c) ret
_s.getwintext(c)
t_noSet=1
EditReplaceSel id(3 hDlg) 0 _s 7
t_noSet=0
ret 1


#sub Set
function hDlg

int- t_noSet
if(t_noSet) ret

int c=sub.FindWnd(hDlg)
if(!c) ret
_s.getwintext(id(3 hDlg))
 _s.setwintext(c)
EditReplaceSel c 0 _s 1|4
