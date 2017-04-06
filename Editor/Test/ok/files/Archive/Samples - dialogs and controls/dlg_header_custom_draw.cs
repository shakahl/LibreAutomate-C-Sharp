\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_header_custom_draw" &dlg_header_custom_draw)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 SysListView32 0x54030001 0x0 0 0 224 114 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "" "" ""

ret
 messages
int h=id(3 hDlg)
sel message
	case WM_INITDIALOG
	TO_LvAddCol h 0 "A" 100
	TO_LvAddCol h 1 "B" 100
	h=SendMessage(h LVM_GETHEADER 0 0)
	if(_winver>=0x501) SetWindowTheme h L"" L"" ;;cannot change background if themed
	 create several brushes
	__GdiHandle-- b1(CreateSolidBrush(0x80e0e0)) b2(CreateSolidBrush(0x80ffc0)) b3(CreateSolidBrush(0x456789))
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
 OutWinMsg message wParam lParam
NMHDR* nh=+lParam
sel nh.idFrom
	case 0
	sel nh.code
		case NM_CUSTOMDRAW
		NMCUSTOMDRAW* cd=+nh
		sel cd.dwDrawStage
			case CDDS_PREPAINT
			FillRect cd.hdc &cd.rc b3
			ret DT_Ret(hDlg CDRF_NOTIFYITEMDRAW)
			
			case CDDS_ITEMPREPAINT
			 out cd.dwItemSpec
			 set background color
			FillRect cd.hdc &cd.rc iif(cd.dwItemSpec&1 b1 b2)
			 set text color
			SetTextColor cd.hdc 0xff0000
			SetBkMode cd.hdc TRANSPARENT
