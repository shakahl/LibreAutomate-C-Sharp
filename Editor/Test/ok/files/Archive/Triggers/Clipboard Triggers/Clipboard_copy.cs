 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int action=wParam
UDTRIGGER& p=+lParam
 out action

sel action
	case 1 ;;Properties dialog init
	goto properties_dlg
	
	case 2 ;;Properties dialog OK
	p.tdata.getwintext(id(3 p.hwnd))
	p.tdata.escape(1); p.tdata-"''"; p.tdata+"''"
	
	case 3 ;;validate trigger string
	str s1(p.tdata) s2
	s1.trim(34); s1.escape(0)
	findrx "" s1 0 128 s2; err out _error.description; ret
	ret 1
	
	case 4 ;;apply all triggers
	int h=win("QM Clipboard Triggers" "#32770")
	if(!h) ;;launch trigger engine
		CCT_table p
		mac "clipboard_copy_triggers"
	else
		int am=WM_APP ;;create table
		if(!p.niids) am+1 ;;destroy h
		SendMessage h am 0 &p
	 
	case 5 ;;icon
	ret GetFileIcon("$qm$\copy.ico")
	
	case 6 ;;help
	QmHelp "IDP_PCRE"
ret

 properties_dlg
str controls = "3"
str e3
e3=p.tdata
e3.trim(34); e3.escape(0)
ret ShowDialog("Clipboard_copy" &Clipboard_copy &controls p.hwnd 1 WS_CHILD)

 BEGIN DIALOG
 0 "" 0x10000048 0x10000 0 0 227 151 ""
 4 Static 0x54000000 0x0 2 2 214 12 "Run when clipboard text matches this regular expression"
 3 Edit 0x54231044 0x200 2 14 212 66 ""
 5 Button 0x54032000 0x0 2 82 20 14 "RX"
 6 Static 0x54000000 0x0 0 130 216 22 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3 ;;show rx error
	s1.getwintext(id(3 hDlg))
	findrx "" s1 0 128 s2; err _s=_error.description
	_s.setwintext(id(6 hDlg))
	
	case 5 RegExpMenu id(3 hDlg)
ret 1
