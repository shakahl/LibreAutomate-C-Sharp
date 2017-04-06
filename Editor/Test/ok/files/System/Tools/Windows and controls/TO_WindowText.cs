 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 1011 1202 1002 1004 1005 10 16 18 1007 1008 1016 1203"
__strt lb3act c1011Err e1202tim cb1002res e1004x e1005y cb10win e16hwn e18rec e1007txt cb1008f e1016mat c1203Con

TO_FavSel wParam lb3act "Find text item (Find)[]Wait for text item (Wait)[]Get text (CaptureToString)[]Get array (Capture)"
cb10win="&Select now[]I have variable(s)[]User selects at run time[]Previous"
cb1008f="&Partial[]Full[]With *?[]Reg. expr."
cb1002res="[]&Move mouse[]Left click[]Right click[]Middle click[]Double click[]Get Acc[]Get WTI"
c1011Err=1
__strt-- t_winStr1 t_winStr2 t_winStr3; RECT-- t_r; int-- t_flags t_hwnd t_selected t_findFlags

if(!ShowDialog("" &TO_WindowText &controls _hwndqm)) ret

str s sout wt f v
int action=val(lb3act)
__strt d wtd

 window and rect
int winCB=val(cb10win)
if(winCB=3) wt="wt"
else
	wtd.VD("-rd WindowText wt" wt)
	if winCB=2
		s=F"{wtd}.InitInteractive"
		if(t_flags) s+F"(0x{t_flags})"
	else
		if(!t_selected) winCB=1
		if winCB=0
			if(t_winStr2.len) t_winStr2=F"{d.VD(`-i int wMain` _s)}={t_winStr2}[]"; t_winStr1.WinReplace(_s)
			s=F"{t_winStr2}{d.VD(`-r int w` e16hwn)}={t_winStr1}{t_winStr3}[]"
			if(t_selected=2 and e18rec.len) s+F"{d.VD(`RECT r; ` e18rec)}SetRect &{e18rec} {t_r.left} {t_r.top} {t_r.right} {t_r.bottom}[]"; else e18rec.VN("0")
		else e16hwn.VN("w"); e18rec.VN("0")
		s+F"{wtd}.Init({e16hwn} {e18rec} 0x{t_flags})"
		sub_to.Trim s " 0 0x0"
	s+"[]"

 capture/find/wait
sel action
	case [0,1] ;;Find, Wait
	int action2=val(cb1002res)
	
	 Find or Wait
	_i=val(cb1008f)|t_findFlags
	sel action
		case 0
			f=F"{wt}.Find("
			int noErr=c1011Err!1 or (action2>=1 and action2<=6) ;;Find flag 0x1000 not necessary with Mouse/GetAcc because it will throw error if WTI 0
			_i|!noErr<<12
		case 1
			f=F"{wt}.Wait({e1202tim.N} "
			_i|val(c1203Con)<<8
	f+F"{e1007txt.S} 0x{_i} {e1016mat.N})"
	sub_to.Trim f " 0x0 0"
	
	 action when item found
	sel action2
		case 0 ;;no action
			if(noErr) s+F"{d.VD(`-rd int found`)}={f}!0"
			else s+f
		case 6 ;;Acc
			s+F"{d.VD(`-iD Acc a` v)}={wt}.GetAcc({f})"
		case 7 ;;WTI
			s+F"{d.VD(`-rid WTI* t` v)}={f}"
		case else ;;Mouse
			s+F"{wt}.Mouse({action2-1} {f})"
	
	if action2>=1 and action2<=6 and (e1004x.len or e1005y.len) ;;offset x y
		s.insert(F" {e1004x.N} {e1005y.N}" s.len-1)
	
	case 2 ;;Get text
	s+F"{d.VD(`-iD str s` v)}={wt}.CaptureToString"
	
	case 3 ;;Get array
	s+F"{wt}.Capture"

 sub_to.TestDialog s action
InsertStatement s

 out sample code to display results
sel action
	case [0,1] sel(action2) case 6 sout=F"out {v}.Name"; case 7 sout=F"out {v}.txt"
	case 2 sout=F"out {v}"
	case 3 sout=F"out[]for _i 0 {wt}.n[][9]{d.VD(`-iD WTI& t` v)}={wt}.a[_i][][9]out {v}.txt"
if(sout.len) out "<><code> sample code, shows how to use results[]%s</code>" sout

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 278 164 "Window Text" "0 10"
 3 ListBox 0x54230101 0x204 4 4 96 38 "act"
 1011 Button 0x54012003 0x0 6 54 94 12 "Error if not found"
 1201 Static 0x54000000 0x0 4 48 46 12 "Wait max, s"
 1202 Edit 0x54030080 0x200 52 46 48 14 "tim" "Max time (seconds) to wait. Then error. Default - infinite."
 1002 ComboBox 0x54230243 0x0 10 97 84 213 "res" "What to do with the found text item.[]Can move mouse to it, click,[]get accessible object at that place,[]get WTI variable (item text, rectangle...),[]or do nothing (use to check if the text exists)."
 1003 Static 0x54000000 0x0 10 116 18 13 "x y"
 1004 Edit 0x54030080 0x200 38 114 26 14 "x" "Offset from top-left of text item rectangle. Default - center."
 1005 Edit 0x54030080 0x200 68 114 26 14 "y" "Offset from top-left of text item rectangle. Default - center."
 10 ComboBox 0x54230243 0x0 118 18 96 213 "win"
 19 Button 0x54032000 0x0 230 18 38 14 "Flags..." "Text capturing options"
 15 Static 0x54000000 0x0 118 38 28 12 "Handle"
 16 Edit 0x54030080 0x200 148 36 24 14 "hwn" "Variable containing window or control handle.[]You can click Select to create/set it, or use an existing variable."
 17 Static 0x54000000 0x0 184 38 58 12 "RECT (optional)"
 18 Edit 0x54030080 0x200 244 36 24 14 "rec" "RECT variable containing rectangle coordinates.[]You can click Select to create/set it, or use an existing variable."
 11 Button 0x54032000 0x0 118 54 96 14 "Select"
 6 Button 0x54032000 0x0 220 54 48 14 "Test" "Shows rectangles of text items in the window.[]Red - visible. Yellow - invisible or not in RECT.[]To hide, click there or activate this dialog.[]Shift+F11 will test any window from mouse."
 14 QM_DlgInfo 0x44000000 0x20000 118 34 150 34 "Will use previous window/rectagle. If used with Find, it searches in text capturing results of previous Find, Wait or Capture instead of capturing again."
 1007 Edit 0x54030080 0x200 118 96 130 14 "txt"
 1015 Button 0x54032000 0x0 250 96 18 14 "RX" "Regular expression menu"
 1008 ComboBox 0x54230243 0x0 118 114 58 213 "f" "What is the specified text.[]Part of item text, full text, text with wildcard characters, or regular expression."
 1009 Button 0x54032000 0x0 178 114 18 14 "..." "More options"
 1001 Static 0x54000000 0x0 212 116 32 13 "Match #"
 1016 Edit 0x54030080 0x200 244 114 24 14 "mat" "1-based match index.[]For example, use 2 for the second matching item."
 1203 Button 0x54012003 0x0 4 62 96 12 "Continuous capturing" "Less CPU, faster response. May be less reliable."
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 54 146 48 14 "Cancel"
 4 Button 0x54032000 0x4 104 146 16 14 "?"
 5 Button 0x54032000 0x0 122 146 48 14 "Test" "Test whether code created by this dialog will work, and how fast"
 9 Static 0x54000000 0x0 176 148 100 13 ""
 1006 Button 0x54020007 0x0 112 84 162 48 "Item text"
 1000 Button 0x54020007 0x0 4 84 96 48 "When found"
 8 Button 0x54020007 0x0 112 4 162 69 "Window and rectangle"
 7 Static 0x54000010 0x20004 0 138 282 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "(0 1) (0 2) 3 3" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\wintext.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	__RegisterHotKey-- t_hk.Register(hDlg 1 MOD_SHIFT VK_F11)
	goto gAction
	
	case WM_ACTIVATE
	if(wParam) WT_ResultsVisualClose
	
	case WM_HOTKEY
	sel wParam
		case 1 mac "sub.Test" "" hDlg 0 0 t_flags
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3 ;;action
	 gAction
	action=TO_Selected(hDlg 3)
	DT_Page hDlg action "(0 1) (0 2) 3 3"
	QmHelp "WindowText.Find[]WindowText.Wait[]WindowText.CaptureToString[]WindowText.Capture" action
	goto gAction2
	
	case CBN_SELENDOK<<16|1002 ;;results CB
	 gAction2
	action=TO_Selected(hDlg 3)
	action2=TO_Selected(hDlg 1002)
	if action<=1
		_i=action2>=1&&action2<=6
		TO_Show hDlg "1003-1005" _i
		TO_Show hDlg "1011" action=0&&!_i
	
	case CBN_SELENDOK<<16|10 ;;window CB
	_i=CB_SelectedItem(lParam)
	TO_Show hDlg "15-18" _i<2
	TO_Show hDlg "19 -14" _i<3
	TO_Show hDlg "11 6" _i=0
	
	case 19 ;;capturing flags
	sub_to.FlagsDialog t_flags "1,WT_SPLITMULTILINE[]2,WT_JOIN[]4,WT_JOINMORE[]8,WT_NOCHILDREN[]16,WT_VISIBLE[]32,WT_REDRAW[]64,WT_SORT[]128,WT_SINGLE_COORD_SYSTEM[]0x400,WT_NOCLIPTEXT[]0x800,WT_GETBKCOLOR" hDlg "Text capturing options" "WindowText.Init"
	
	case 11 ;;Select
	__MinimizeDialog m.Minimize(hDlg)
	_i=CaptureWindowAndRect(t_hwnd t_r); if(!_i) ret
	t_selected=_i
	RecGetWindowName(t_hwnd &t_winStr1 0 &t_winStr2 &t_winStr3)
	if(t_winStr3.len) t_winStr3.s-" ;;"
	TO_SetText "w" hDlg 16 6; if(_i=2) TO_SetText "r" hDlg 18 6; else TO_SetText "" hDlg 18 4
	
	case 1009 ;;find flags
	sub_to.FlagsDialog t_findFlags F"4,Case insensitive (4)[]8,+ invisible (8)" hDlg
	
	case 1015 ;;RX
	CB_SelectItem id(1008 hDlg) 3
	RegExpMenu id(1007 hDlg)
	
	case 4 ;;?
	QmHelp "WindowText help"
	
	case 6 ;;Test window
	goto gTest
	
	case 5 ;;Test selected action
	action=TO_Selected(hDlg 3)+1
	goto gTest
ret 1

 gTest
if(!t_selected) mes "Window not selected."; ret
RECT* rp; if(t_selected=2) rp=&t_r
sel action
	case [1,2]
	str findTxt; int findFlags findIndex
	findTxt.getwintext(id(1007 hDlg))
	if(!findTxt.len) mes "Text empty."; ret
	findFlags=TO_Selected(hDlg 1008)|t_findFlags
	findIndex=GetDlgItemInt(hDlg 1016 0 0)
mac "sub.Test" "" hDlg t_hwnd rp t_flags action findTxt findFlags findIndex


#sub Test
function hDlg hwnd RECT&r flags action $findTxt findFlags findIndex ;;action 0 capture/visual, 1,2 find, 3 get text, 4 get array

RECT _r; if(&r) _r=r; &r=_r

if hwnd
	act GetAncestor(hwnd 2)
	0.3
else
	if(WT_ResultsVisualClose) ret
	hwnd=child(mouse); if(!hwnd) hwnd=win(mouse)
	int isHK=1

WindowText x.Init(hwnd r flags)

long t1=perf
str s
WTI* t

sel action
	case [0,4] x.Capture
	case [1,2] t=x.Find(findTxt findFlags findIndex)
	case 3 s=x.CaptureToString

x.End

double t2=perf-t1/1000.0; t2=Round(t2 t2<10)
_s=F"Time: {t2} ms"
_s.setwintext(id(9 hDlg)); err

sel action
	case 0 WT_ResultsVisual x.a x.n hwnd flags
	case 4 out; for(_i 0 x.n) out "%i. %s" _i x.a[_i].txt
	case 3 out; out s
	case [1,2]
	if t
		if(!hid(id(1004 hDlg))) str sx.getwintext(id(1004 hDlg)) sy.getwintext(id(1005 hDlg))
		if(sx.len or sy.len) x.Mouse(0 t val(sx) val(sy)); else x.Mouse(0 t)
		
		&r=iif(t.flags&WTI_INVISIBLE t.rt t.rv)
		int w=iif((flags&WT_SINGLE_COORD_SYSTEM) hwnd t.hwnd)
		DpiMapWindowPoints w 0 +&r 2
		__OnScreenRect osr
		rep(3) osr.Show(0 r); 0.25; osr.Show(3 r); 0.25
		
		mou
	else
		int notFound=1

err+ mes _error.description

if(!isHK) act hDlg; err

if(notFound) mes "Not found."
