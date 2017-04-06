
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ListBox 0x54230101 0x200 11 7 88 63 ""
 4 Button 0x54012003 0x0 104 8 48 10 "Check"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040304 "*" "" "" ""

str controls = "3 4"
str lb3 c4Che
lb3="test 1[]test 2[]test 3"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int hlb=id(3 hDlg) ;;list
	SetFocus hlb
	SetWindowSubclass(hlb &sub.WndProc_Subclass 1 0)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case LBN_DBLCLK<<16|3
	int i=LB_SelectedItem(lParam)
	out F"selected test {i+1}"
ret 1


#sub WndProc_Subclass ;;menu File -> New -> Templates -> Wndproc -> WndProc_Subclass
function# hwnd message wParam lParam uIdSubclass dwRefData

sel message
	case WM_GETDLGCODE
	sel(wParam) case [VK_RETURN,VK_SPACE] ret DLGC_WANTALLKEYS ;;please send me WM_KEYDOWN instead of closing the dialog
	
	case WM_KEYDOWN
	sel(wParam) case [VK_RETURN,VK_SPACE] SendMessage GetParent(hwnd) WM_COMMAND LBN_DBLCLK<<16|GetDlgCtrlID(hwnd) hwnd; ret ;;to use the same code as for double-click

int R=DefSubclassProc(hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hwnd &sub.WndProc_Subclass uIdSubclass)

ret R
