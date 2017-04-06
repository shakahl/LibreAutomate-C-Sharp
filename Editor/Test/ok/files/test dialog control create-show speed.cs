 /exe


 BEGIN PROJECT
 main_function  Macro2738
 exe_file  $my qm$\Macro2738.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {91BD2E45-B310-474F-B692-FC40049C2410}
 END PROJECT


str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 424 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_TIMER
	PN
	PO 0 _s
	_s.setwintext(hDlg)
	KillTimer hDlg 1
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 g1
ARRAY(int) a.create(8)
int i
PF
for i 0 a.len
	a[i]=CreateControl(0 "Static" F"{i}" 0 0 i*20 100 20 hDlg 0)
	PN
SetTimer hDlg 1 1 0

