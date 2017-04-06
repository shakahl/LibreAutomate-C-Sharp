\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

def EN_LINK 0x070b
def ENM_LINK 0x04000000
def EM_AUTOURLDETECT (WM_USER+91)
type ENLINK NMHDR'nmhdr msg wParam lParam CHARRANGE'chrg

str controls = "3"
str rea3
rea3="simple text[]http://www.quickmacros.com[]"
if(!ShowDialog("dlg_rich_text2" &dlg_rich_text2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 4 4 216 104 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	int re=id(3 hDlg)
	 let autodetect URL
	SendMessage(re EM_AUTOURLDETECT 1 0)
	 enable events
	SendMessage(re EM_SETEVENTMASK 0 ENM_LINK)
	
	DT_Init(hDlg lParam)
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
 messages3
NMHDR* n=+lParam
sel n.code
	case EN_LINK
	ENLINK* el=+n
	if(el.msg=WM_LBUTTONUP)
		TEXTRANGE tr
		tr.chrg=el.chrg
		tr.lpstrText=_s.all(tr.chrg.cpMax-tr.chrg.cpMin)
		_s.fix(SendMessage(id(3 hDlg) EM_GETTEXTRANGE 0 &tr))
		run _s
