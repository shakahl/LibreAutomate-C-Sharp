 /Dialog_Editor
function# $caption $text [hwndOwner] [flags] ;;flags: 1 modeless, 2 QM-format, 4 text is file.

 Text box (dialog).
 Returns 0 if modal, window handle if modeless, -1 on error.

 caption - dialog title bar text. Default: "QM Textbox".
 text - text.
 hwndOwner - handle of owner window or 0.
 flags:
   1 - modeless dialog. Does not wait until closed.
     This thread must process messages. If hangs, insert 'opt waitmsg 1' before.
   2 - text is formatted as QM code.
     To display text of a QM item, pass item id in the high-order word of flags: flags=MakeInt(2 qmitem("name")). Then text is not used and can be "".
     Unavailable in exe.
   4 - text is file path. Displays file text.
     If rtf file, displays in rich text format.

 EXAMPLES
  show macro text:
 str s.getmacro("Macro1")
 ShowText "" s

  show text from file:
 ShowText "ReadMe" "$desktop$\readme.txt" 0 4

  show rich text from file:
 ShowText "Document" "$desktop$\document.rtf" 0 4


str dd=
 BEGIN DIALOG
 2 "" 0x90CF0A48 0x100 0 0 350 220 ""
 3 RichEdit20W 0x55200844 0x204 0 0 350 220 ""
 2 Button 0x54000001 0x4 0 161 0 0 ""
 END DIALOG

str controls = "0 3"
str Dlg Edit3

Dlg=iif(empty(caption) "QM Textbox" caption)
if(flags&4) Edit3.from("&" text); else Edit3=text

ret ShowDialog(dd &sub.DlgProc &controls hwndOwner flags&1 0 0 flags)


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	SetFocus(id(2 hDlg))
	int flags=DT_GetParam(hDlg)
	if(flags&2) SendMessage(_hwndqm WM_USER+26 flags id(3 hDlg))
	
	case WM_COMMAND
	sel wParam
		case [IDOK,IDCANCEL] DT_Cancel hDlg
	ret 1
	
	case WM_SIZE
	RECT r; GetClientRect(hDlg &r)
	MoveWindow(id(3 hDlg) 0 0 r.right r.bottom 1)
