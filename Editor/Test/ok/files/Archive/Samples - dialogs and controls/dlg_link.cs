\Dialog_Editor

 SysLink control.
 The control shows text with links that can open web pages, files, or execute any other code.
 Link click notifications can be received in two ways:
   The easy way: In recent QM versions, if the link does not have id/href attributes, on click the dialog procedure receives WM_COMMAND message where wParam is link control id and lParam is 0-based index of the link in that control.
   The standard way: In any QM version, on click the dialog procedure receives WM_NOTIFY message with code NM_CLICK. It then can get link control id, link index and link attribute text. More info in MSDN Library.


str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 4 SysLink 0x54030000 0x0 32 12 96 20 "<a>Zero</a> <a>One</a>[]<a href=''two''>Two</a> <a href=''three''>Three</a>"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040304 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 Link text can be set in dialog definition or with setwintext:
	 _s="<a href=''go''>Google</a> <a href=''bi''>Bing</a>"
	 _s.setwintext(id(4 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3

ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 4 ;;link clicked
	out "link index=%i" lParam
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4 ;;syslink control
	NMLINK* di=+nh
	sel nh.code
		case [NM_CLICK,NM_RETURN] ;;link clicked
		str url.ansi(&di.item.szUrl)
		out "link index=%i, text=%s" di.item.iLink url
