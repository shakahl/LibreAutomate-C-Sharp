\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
rea3="a[]b[]c[]d[]e[]f[]g[]h[]"
if(!ShowDialog("ken_gray_dialog" &ken_gray_dialog &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 3 RichEdit20A 0x54233044 0x200 106 2 96 48 ""
 2 Button 0x54030001 0x4 170 116 48 14 "Cancel"
 4 Button 0x54032000 0x4 118 116 48 14 "Save"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	int hedit=id(3 hDlg)
	SetFocus hedit
	SendMessage hedit EM_SETSEL -2 -2
	 SendMessage hedit WM_VSCROLL 7 0 ;;def SB_BOTTOM 7
	ret
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 ;;Save
	str s.getwintext(id(3 hDlg))
	s.setfile("C:\qm\projects.txt")
	
	case IDCANCEL DT_Cancel hDlg
ret 1
