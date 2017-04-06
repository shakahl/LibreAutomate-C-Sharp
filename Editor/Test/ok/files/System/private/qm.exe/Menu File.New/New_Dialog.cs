 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 284 166 "Custom dialog"
 3 ListBox 0x54230101 0x204 8 8 88 48 "Act"
 1003 Button 0x54032009 0x4 112 8 70 12 "Create function"
 1002 Button 0x54002009 0x4 112 24 70 12 "Add dialog here"
 1004 Edit 0x54030080 0x204 184 8 92 14 "Fun"
 6 Button 0x54012003 0x0 112 44 164 13 "Add sample controls, code and comments"
 1 Button 0x54030001 0x4 8 144 48 14 "OK"
 2 Button 0x54030000 0x4 60 144 48 14 "Cancel"
 4 Button 0x54032000 0x4 112 144 18 14 "?"
 7 QM_DlgInfo 0x54000000 0x20000 8 64 268 68 ""
 5 Static 0x54000010 0x20004 0 136 294 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040108 "*" "0" "" ""

str sm st sl="New dialog[]New smart dialog[]New multipage dialog"
if(findrx(sm.getmacro __S_RX_DD)>=0) sl+"[]Edit this dialog"
else if(wParam>2) ret

str controls = "3 1003 1002 1004 6"
__strt lb3Act o1003Cre o1002Add e1004Fun c6Add

TO_FavSel wParam lb3Act sl
o1003Cre=1

if(!ShowDialog(dd &New_Dialog &controls _hwndqm)) ret

e1004Fun.VN("Dialog")

int i action=val(lb3Act)
if(action<0 or action>2) goto gDE

str fn.getl("Dialog[]DialogEx[]DialogMultiPage" action)
if(c6Add=1 and action<2) fn+"_sample"

if o1002Add=1 ;;add dialog here
	sm.getmacro
	st.getmacro(fn)
	if sm.len
		sm+"[]"
		if(!findrx(sm "[ ;]?[/\\]Dialog_Editor[]")) st.getl(st 1 2)
		if findrx(sm __S_RX_DD)>=0 ;;if a dialog definition already exists, add new sub-function
			for(i 2 1000000) if(findrx(sm F"(?m)^#sub +Dialog{i}\b")<0) break ;;unique name
			if(action) st.findreplace("DlgProc" F"DlgProc{i}" 2)
			sm+F"[]#sub Dialog{i}[]function# [hwndOwner][]"
			st.replacerx("(ShowDialog\(\S+ \S+ \S+?)\)\) ret[]" "$1 hwndOwner)) ret[][]ret 1[]" 4)
	sm+st
	sm.setmacro
else
	newitem(e1004Fun st fn "" "" 12)

 gDE
mac "Dialog_Editor" "" action<=2

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\dialog.ico")) ret wParam
sel message
	case WM_INITDIALOG
	QMITEM q; qmitem "" 0 q; sel(q.itype) case [0,1,6] case else TO_Enable hDlg "1002" 0 ;;cannot "add here" in menu etc
	goto g11
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 QmHelp "IDH_DIALOG_EDITOR"

	case [LBN_SELCHANGE<<16|3,1001]
	 g11
	i=TO_Selected(hDlg 3)
	DT_Page hDlg i "0 0 0 1"
	TO_Show hDlg "6" i<2
	
	sel i
		case [0,1,2]
		if(i) st="Creates initial dialog definition and dialog procedure sub-function. In the dialog procedure later you can add code to execute in response to events, for example when user clicks a button."
		else st="Creates initial dialog definition (BEGIN DIALOG ... END DIALOG) and opens it in the Dialog Editor."
		st+" Also inserts code to show the dialog.[][]When you click Save in the Dialog Editor, it updates the dialog definition and ShowDialog code (it is set in Dialog Editor options by default)."
		case 3 st="Opens the dialog definition in the Dialog Editor.[][]Other ways:[]1. Click 'Dialog Editor' button or menu item in QM window.[]2. If current macro/function begins with ' \Dialog_Editor' line, the Run button opens it in the Dialog Editor. To make the function/macro executable, delete space before '\Dialog_Editor'."
	st.setwintext(id(7 hDlg))
ret 1
