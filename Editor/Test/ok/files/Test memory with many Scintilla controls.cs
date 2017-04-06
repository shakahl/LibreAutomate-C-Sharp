 /exe
LoadLibrary("Q:\app\Catkeys\Editor\SciLexer32.dll")

 0 1.2
 1 1.6
 6 1.7
 20 2.5
 30 2.9
 40 3.6
 50 3.9
 

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 800 400 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

int- x

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_LBUTTONUP
	 out F"{x} {x%1000} {x/1000}"
	int h=CreateControl(0 "Scintilla" 0 0 x%1000 x/1000*100+50 90 90 hDlg 3)
	x+100
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Macro2732
 exe_file  $my qm$\Macro2732.qmm
 flags  6
 guid  {087A7F8A-AD24-4905-8228-A10870DFB323}
 END PROJECT
